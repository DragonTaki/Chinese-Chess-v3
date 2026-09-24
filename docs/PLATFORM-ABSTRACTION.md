# 平台轉換層設計(Adapter / Ports-and-Adapters)

目的:把所有直接呼叫 `System.Drawing`／`System.Windows.Forms` 的地方,改成呼叫
一組自訂介面,現有的 WinForms/GDI+ 實作變成「其中一種後端」。之後要支援
其他平台,只要多寫一份實作這些介面的後端,`Engine/UI`、`Game/UI`、
`StarAnimation` 的程式碼一行都不用動。

`Engine/Timing/` 已經是這個模式了(`ITimerProvider` 介面 + `TimerManager`
WinForms 實作),可以直接參考它的形狀。

## 完整盤點(從實際 build 警告撈出來的,不是用猜的)

實際被直接呼叫的 Windows-only 成員,依用途分類:

| 分類 | 成員 |
|---|---|
| 繪圖表面 | `Graphics`(`DrawEllipse`／`DrawLine`／`DrawPath`／`DrawRectangle`／`DrawString`／`FillEllipse`／`FillPath`／`FillRectangle`／`FillRegion`／`SetClip`／`ResetClip`／`MeasureString`／`Dispose`／`FromHwnd`／`FromImage`／`SmoothingMode`／`InterpolationMode`／`PixelOffsetMode`／`TextRenderingHint`) |
| 筆刷 | `Brush`／`SolidBrush`／`LinearGradientBrush`／`Brushes.Black`／`Brushes.DarkRed`／`ColorBlend` |
| 筆 | `Pen`(`Alignment`／`DashStyle`) |
| 路徑 | `GraphicsPath`(`AddArc`／`AddBezier`／`AddLine`／`Clone`／`CloseFigure`／`StartFigure`／`Transform`) |
| 文字 | `Font`／`FontFamily`／`FontStyle`／`FontCollection`／`PrivateFontCollection`／`SystemFonts.DefaultFont`／`StringFormat`／`StringAlignment`／`StringFormatFlags`／`StringTrimming`／`ContentAlignment` |
| 幾何輔助 | `Matrix`(`Shear`／`Translate`)、`Region`(`Intersect`)、`GraphicsUnit` |
| 點陣圖 | `Bitmap`(`UITextBox.cs` 用來量文字尺寸) |
| 視窗/控制項 | `Form`(`ClientSize`／`StartPosition`／`Text`／`OnPaint`)、`Control`(`DoubleBuffered`／`Height`／`Width`／`MouseDown`／`MouseUp`／`MouseMove`／`MouseClick`／`MouseWheel`／`Invalidate`／`SetStyle`)、`ControlStyles`、`FormStartPosition`、`PaintEventArgs.Graphics`、`Application.Run`／`Exit` |
| 滑鼠輸入 | `MouseEventArgs`(`Location`／`X`／`Y`／`Delta`) |
| 計時器 | 已經處理好了,見上面 |

`Color`、`PointF`、`SizeF`、`RectangleF`、`Point`、`Size` **不需要包**——這些是
純資料結構,不是 GDI+ 控制代碼,查證過在 .NET 8+ 上任何平台都能用(`Vector2F.cs`
已經在跟它們互轉,從沒被 `CA1416` 標記過)。

## 建議的介面形狀

```
Engine/Platform/
  IGraphics.cs        繪圖表面:FillRect/DrawRect/FillEllipse/DrawEllipse/
                       FillPath/DrawPath/DrawString/MeasureString/SetClip/
                       ResetClip,以及 SmoothingMode 等品質選項
  IBrush.cs            + SolidBrush 實作、LinearGradientBrush 實作
  IPen.cs
  IGraphicsPath.cs      AddArc/AddBezier/AddLine/StartFigure/CloseFigure/Transform
  IFont.cs / IFontFamily.cs / IFontCollection.cs
  IStringFormat.cs
  IMatrix.cs / IRegion.cs
  IWindow.cs            對應 Form:ClientSize/Text/StartPosition,以及
                       Paint 事件
  IMouseEvent.cs        對應 MouseEventArgs:Location/X/Y/Delta
  WinForms/            現有 GDI+/WinForms 實作,搬進來當「後端 #1」
    WinFormsGraphics.cs
    WinFormsWindow.cs
    ...
```

`Engine/UI`、`Engine/GraphicsUtils`、`Engine/Styles`、`Game/UI`、
`StarAnimation/Renderers` 裡所有 `OnRender(Graphics g, ...)` 這類簽章,改成
`OnRender(IGraphics g, ...)`;`MouseEventArgs e` 改成 `IMouseEvent e`。

## 規模與建議順序

這是真的工程量,不是小改動——粗估用到這些型別的呼叫點:

```
Color            98 次（不用包)
MouseEventArgs    74 次
Graphics          73 次
Font              53 次
SolidBrush        27 次
Pen               20 次
Brush             19 次
GraphicsPath      18 次
FontFamily        13 次
StringFormat      10 次
Matrix             8 次
LinearGradientBrush 6 次
Form               2 次
```

### 已完成:介面 + WinForms 後端(commit `ca4545e`)

`Engine/Platform/`(9 個介面檔)＋ `Engine/Platform/WinForms/`(10 個後端實作檔)
已經建好,並且完整驗證過:0 error,警告從 305 → 400,多出來的 95 個警告
**全部集中在新建的 `WinForms/` 資料夾**,舊檔案一個都沒受影響。這一步是純新增,
零風險,可以直接 revert。

### 修正:「先做 Font/Brush/Pen,再做 Graphics」這個分階段假設是錯的

實際動手接 `Engine/Styles/` 時發現:Font/Brush/Pen **沒辦法真的獨立於
Graphics 先做完**。原因是它們永遠是在同一個 Draw 呼叫點被一起消費的——
例如 `IBoxDrawStyle.Draw(Graphics g, LayoutF bounds)` 內部要用
`BackgroundBrushFactory.Create(bounds)` 產生的 brush 去呼叫
`g.FillPath(brush, path)`。如果 `Create()` 回傳改成 `IBrush`,但 `g` 還是
原生 `Graphics`,兩者就接不起來——`Graphics.FillPath` 只吃真正的
`System.Drawing.Brush`,不吃我們包的 `IBrush`。

也就是說,只要有任何一個消費點的簽章還沒換成 `IGraphics`,那個消費點用到的
`IBrush`/`IFont`/`IPen` 就是「包了但用不到」的狀態。而 `Graphics` 參數的
傳遞鏈是:`UIRenderer<T,H,R>.OnRender(Graphics g, ...)`(base class)→
每一個具體 Renderer 覆寫 `OnRender` → 呼叫 `IBoxDrawStyle.Draw(g, ...)`/
`IButtonDrawStyle.Draw(g, ...)`。這條鏈只要有一段沒換,`Engine/Styles/`
就換不完。

**修正後的認知**:這不是「分批做,批次之間互不影響」,而是「一旦要接通
任何一條消費鏈,就要一次把那條鏈從 `UIRenderer` 基底一路換到最外層的具體
Renderer」。原本以為的階段 1(Font/Brush/Pen)實際上包含了階段 2
(`IGraphics`)的大部分工程量,兩者綁在一起。

### 建議做法(取代原本的四階段規劃)

按「消費鏈」而非「型別種類」分批,每接通一條鏈就是一次完整、可獨立驗證、
可獨立 revert 的 commit:

1. `Engine/Styles/` 的樣式系統一條鏈:`IBoxDrawStyle`／`IButtonDrawStyle`／
   `SingleBorderRoundedStyle`／`DoubleBorderRoundedStyle`／
   `InwardCornerDialogStyle`／`FontManager`／`StyleHelper`／
   `IBrushFactory`／`SolidBrushFactory`／`LinearGradientBrushFactory`，
   加上直接用到它們回傳型別的 `Game/UI/Constants/UILayoutStyles.cs`、
   `Game/UI/Boards/UIBoardStyles.cs`。
2. `UIRenderer`／`UIContainerRenderer` 等 base class 的 `OnRender` 簽章。
3. 逐一把 `Engine/UI/Core/Renderers/`、`Game/UI/*Renderer.cs`、
   `StarAnimation/Renderers/` 底下每個覆寫 `OnRender` 的具體類別換掉——
   這批最多,建議每個檔案（或每組相關檔案）一個 commit。
4. `Engine/GraphicsUtils/GraphicsPaths/`（`IGraphicsPath`／`IMatrix`）。
5. `IWindow`／`IMouseEvent`（`Launcher/`、輸入處理）——最後做,因為要等
   `IGraphics` 全部接通，`MainForm` 才有東西可以真正透過介面繪圖。

規模比原本估計的更大——光是第 1 項就會连带牽動十幾個檔案。先確認範圍跟
優先度再繼續動手。

### 已完成:整條繪圖鏈全部接通(commit `3c08538`)

上面第 1-4 項實際上沒辦法真的拆成獨立 commit——原因見前一段的修正說明,
虛擬方法簽章一改,沒同步改到的覆寫會靜默失效、不是編譯錯誤,所以只能一次
把整條鏈做完才 build 驗證。實際涵蓋:

- `Engine/Platform/GraphicsBackend.cs`(新增)—— 靜態持有目前使用中的
  `IGraphicsFactory`,由 `Launcher/Program.cs` 在 `Main()` 最開頭設定
  (必須在任何 `Engine.Styles`／`Game.UI.Constants` 的靜態初始化跑之前)。
- `UIRendererBase`／`UIRenderer`(3 個變體)／`UIContainerRenderer`／
  `CompositeRenderer` 的 base class 簽章。
- `UIElement.Draw`／`UIElementBase.Draw` 的抽象簽章。
- `Engine/Styles/` 全部 10 個檔案(如上面第 1 項所列)。
- `Engine/GraphicsUtils/`(`GraphicsHelper.cs` 加 4 個 `GraphicsPaths/` 檔案)。
- 11 個具體 Renderer 覆寫:`UILabelRenderer`、`UIMenuRenderer`、
  `UITextBoxRenderer`、`UIOverlayMask`(內部 Renderer)、`UITextBox`(內部
  `UITextLineRenderer`)、`UIPieceRenderer`、`UIBoardRenderer`、
  `UIInfoBoardRenderer`,以及對話框的 `UIConfirmDialogRenderer`。
- `StarAnimation` 整條渲染鏈:`StarAnimationApp`、`MainRenderController`、
  4 個 Controller、3 個 Renderer。
- `UILabel`／`UIButton`／`UITextBox` 等元素類別上 `Font`／`Brush`／
  `ContentAlignment` 型別的公開屬性,以及 `PieceSettings`、
  `UIInfoBoardSettings`、`UILoggerBoxSettings`、`UILayoutStyles`、
  `UIBoardStyles` 這些拿著這些型別當常數的設定檔。
- `Launcher/MainForm.cs` 的 `OnPaint`——在最外層把 `e.Graphics` 包一次成
  `WinFormsGraphics`,往下全部走 `IGraphics`。

**驗證結果**:0 error,警告從 400 降到 **150**——降下去的都是因為呼叫點
不再直接碰 GDI+ 型別,消失的警告是實際生效的訊號,不是巧合。剩下的 150 個
警告全部落在預期範圍:`Engine/Platform/WinForms/` 後端本身、故意留到最後
的滑鼠輸入與視窗部分(見下面「還沒做」清單),以及一個確認過整個專案零
引用的死碼設定檔(`Engine/Configs/EngineSettings.cs`,沒有動它,只是記錄)。

### 已完成:輸入/視窗層接通,`net9.0`(非 Windows)整條引擎程式碼零錯誤

補上 `IMouseEvent`／`IWindow`,以及一個不自己跑計時器、改由外部 host 每幀
呼叫 `Tick(dt)` 的 `Engine/Timing/ManualTimerProvider.cs`(給 Silk.NET 這種
「視窗自己有 Update/Render 事件迴圈」的後端用,避免跟原本的
`System.Windows.Forms.Timer` 版本搶著跑)。涵蓋:

- `UIInputManager.OnMouseDown/Move/Up/Click/Wheel` 簽章改用 `IMouseEvent`,
  原本內嵌的 5 個 WinForms 專用轉接方法搬到新的 `WinFormsInputAdapter`。
- `Engine/UI/Core/` 底下所有處理滑鼠事件的類別(handler/element/router 等
  約 16 個檔案)`MouseEventArgs` 一律改 `IMouseEvent`。
- `UIRootNode.MainWindow`:`Form?` 改 `IWindow?`。
- `Game/UI/Menus/MainMenu/UIMainMenuHandler.cs` 的 `Application.Exit()`
  改走新的 `Engine.Platform.AppControl.ExitCallback`(委派,由各後端在
  `Main()` 設定自己的結束方式)。
- `Engine/Configs/EngineSettings.cs` 裡確認過零引用的 `Font DefaultScrollTextFont`
  常數,改成 `IFont`(維持不動、只是跟著換型別,見前面「不能移除」規則)。

**驗證結果**:`net9.0-windows` 維持 0 error、警告 400→**135**(又降了 15
個,同樣是滑鼠輸入呼叫點不再碰 `MouseEventArgs` 的訊號)。`net9.0`(非
Windows)在這一步之後**只剩 1 個錯誤**:`CS5001`(缺進入點)——代表
`Engine/`、`Game/`、`StarAnimation/` 三層的程式邏輯本身,已經 100% 能在
非 Windows 平台編譯過。

### 已完成:第二個後端(SkiaSharp + Silk.NET),macOS 真的能編譯、能執行

`Engine/Platform/Skia/`——實作全部 10 個 `Engine/Platform` 介面,對應
WinForms 後端的每一個檔案(`SkiaBrush`／`SkiaPen`／`SkiaFontFamily`／
`SkiaFont`／`SkiaGraphicsPath`／`SkiaMatrix`／`SkiaRegion`／
`SkiaStringFormat`／`SkiaGraphics`／`SkiaGraphicsFactory`),加上
`SilkWindow`(`IWindow`)、`SilkMouseEvent`(`IMouseEvent`)、
`SilkInputAdapter`(轉接 Silk.NET `IMouse` 事件,對應
`WinFormsInputAdapter`)。API 形狀先用一次性的 reflection 腳本
(`dotnet run` + `System.Reflection`)在本機驗證過 SkiaSharp 4.152.1 與
Silk.NET 2.23.0 的真實簽章,再動手寫,避免用猜的接錯。

`Launcher.Cross/`(新增,對應 `Launcher/`)——`CrossPlatformApp.cs`
(對應 `MainForm.cs`)：用 `Silk.NET.OpenGL` 的 `GL.GetApi(window)` 建立
GL binding、`GRGlInterface.Create()` + `GRContext.CreateGl()` 建立
GPU-backed 的 Skia 繪圖 context,每一幀用當時的 framebuffer(`FBO 0`)
建一個 `GRBackendRenderTarget`、包成 `SKSurface`,畫完
`surface.Canvas.Flush()` + `_grContext.Flush()`,Silk.NET 視窗自己負責
`SwapBuffers`。`Program.cs`(對應 `Launcher/Program.cs`)是完全對應的
DI 註冊,只差 `GraphicsBackend.Factory = new SkiaGraphicsFactory()` 跟
用 `Window.Create()` 取代 WinForms 的 `Form`。`UIInitializer.cs`／
`ServiceCollectionExtensions` 因為兩個進入點分別被對方 TFM 排除、無法共用
檔案,是內容相同的獨立副本。

一個小地方跟 WinForms 版不同:`IWindow.Invalidate()` 在 Silk 後端是空
操作——Silk.NET 的 `Render` 事件本來就每幀都會觸發,沒有「只在需要時才
重繪」這回事,所以不需要真的做什麼。

`Chinese-Chess-v3.csproj` 加了 `<RollForward>LatestMajor</RollForward>`
(只套用在非 Windows TFM)——這台機器只裝了 .NET 10 執行環境,沒有
9.0,沒開這個的話會直接因為找不到執行環境而啟動失敗,跟程式碼本身無關。

**驗證結果**:`net9.0` 編譯 0 error、8 warning(剩下的都是 SkiaSharp
`SKPath.MoveTo/LineTo/ArcTo/CubicTo/Close` 這幾個「建議改用
`SKPathBuilder`」的過時 API 警告——功能沒問題,只是新版函式庫建議的寫法
不同,列在這裡當已知技術債,不影響能不能動)。實際 `dotnet run` 執行
`Chinese-Chess-v3.dll`:DI 容器建置成功、UI 樹(主選單/新對局/讀取對局/
規則設定等選單、按鈕、捲動容器)全部照原本邏輯初始化完畢,console log
可以看到選單被實際操作(`MaunMenu: selected: ...`);macOS 的
`System Events` 行程列表裡也看得到這個 `dotnet` 行程被列為非背景(前景)
應用程式,代表視窗確實有被建立,不是 headless 跑完就結束。

**還沒能在這個環境驗證的部分**:這個沙盒環境本身連 `osascript` 的輔助
使用權限都被擋掉(`osascript is not allowed assistive access`),沒辦法
用程式化的方式截圖、量測視窗內容或確認畫面實際渲染的樣子,只能確認「有
沒有當掉、進程有沒有活著、有沒有被系統列成一個視窗程式」。另外這個進程
在沒有人為操作的情況下,運行數秒後就自行以 exit code 0 正常結束(不是
崩潰、沒有例外堆疊)——研判跟這個沙盒環境對背景啟動之 GUI 程式的視窗
生命週期限制有關,不是程式碼邏輯錯誤(建置警告與執行 log 都找不到對應的
異常訊號)。這一步之後需要在一般的 macOS 使用者工作階段(不是這個受限的
沙盒)實際跑一次 `dotnet run --project Chinese-Chess-v3.csproj -f net9.0`
才能真正確認畫面渲染正確、滑鼠互動正常。

### 已修正:實機測試後發現的三個 bug

你在自己機器上實際跑起來後回報:中文變方框、背景/物理動畫完全不會動、
畫面解析度沒跟著視窗大小走。三個都是真的 bug,根因與修法：

1. **中文方框**——`UILayoutStyles`／`PieceSettings`／`UIInfoBoardSettings`
   這些靜態類別的欄位會呼叫 `StyleHelper.GetFont("NotoSerif"/"MoeLI", ...)`，
   但它們的靜態建構子被 `Program.Main()` 裡的
   `DefaultStyles.DefaultButtonStyle = UILayoutStyles.MainMenu.Button.Style;`
   提早觸發，那時候 `FontManager.LoadFonts()` 根本還沒跑（原本放在
   `MainForm`/`CrossPlatformApp.OnLoad` 裡，時機太晚）。結果每個 CJK 字型
   key 都 fallback 成「找一個叫這個名字的系統字型」，Skia 的 fallback
   對未知字型名稱沒有中文字。這個 bug 在 Windows 上其實也存在，只是
   Windows 的字型連結機制運氣好幫忙補上了中文字型。修法：把
   `FontManager.LoadFonts()` 移到兩個進入點 `Main()` 最前面。
2. **物理/背景不會動 + 解析度沒跟視窗**——同一個根因：Retina 螢幕上
   `CrossPlatformApp` 用實際 GPU surface 的 `FramebufferSize`（物理像素）
   畫圖，但 `GlobalWindow`（背景動畫的邊界依據）只在啟動時設成 `Size`
   （邏輯像素）且從沒更新過，兩者差了螢幕縮放倍率，且視窗大小變化時
   完全沒有同步。修法：全部改用 `FramebufferSize`，並在
   `FramebufferResize` 事件時同步更新，滑鼠座標也换算成物理像素。

三項都已 build 驗證（兩個 TFM 都 0 error，警告數不變）並 commit + push
（`8b487ee`）。

### 已完成:畫面解析度縮放系統（`GlobalViewport`）

延續上面的討論，進一步處理「視窗可以任意縮放，但畫面內容比例要維持
不變」這個需求。設計:

- `Engine/Globals/GlobalViewport.cs`（新增）——把 UI 內容自己的原生尺寸
  （`DesignSize`，即 `UILayoutConstants.DesignSize` = MainMenu + Board +
  Sidebar 三者尺寸加總，目前是 1500×840，跟原本設計完全一樣，沒有重新
  設計任何版面）對應到實際視窗大小：`Scale` 取寬高縮放比例中較小值（等比例
  縮放，內容絕不變形），`Offset` 讓內容置中，兩軸不滿的部分留白（letterbox
  /pillarbox）。跟 `GlobalWindow`（仍然回報實際視窗像素）刻意分開——背景
  StarAnimation 繼續讀 `GlobalWindow`，滿版鋪到視窗邊緣（含留白區域也蓋到），
  UI 內容（選單/棋盤/對話框）改讀 `GlobalViewport`，永遠在自己的 1500×840
  座標系裡。
- `IGraphics.PushTransform(scale, offsetX, offsetY)` / `PopTransform()`（新增
  介面方法，兩個後端都已實作）——在畫 UI 內容前推入一個「平移＋等比例縮放」
  的繪圖狀態，畫完再還原，這樣所有現有的版面程式碼完全不用改，一行都沒動。
- 滑鼠座標比照辦理：Skia 後端在 `SilkInputAdapter` 內把「邏輯座標→實體
  framebuffer 像素→內容座標」兩段轉換串起來；WinForms 後端因為沒有實體
  framebuffer 這層，`WinFormsMouseEvent` 直接做「視窗像素→內容座標」一段
  轉換。
- 視窗預設啟動大小改成 1920×1080（`UILayoutConstants.DefaultWindowSize`，
  一般桌機的標準解析度），內容照樣用 1500×840 設計、放大鋪滿；最小視窗
  大小鎖定 960×540（`UILayoutConstants.MinimumWindowSize`，同比例的一半，
  防止縮到內容看不清楚）——WinForms 用 `Form.MinimumSize` 原生鎖定，Silk.NET
  沒有對應的原生 API，改成在 `Resize` 事件裡手動夾住。

WinForms（`MainForm.cs`）跟 Skia（`CrossPlatformApp.cs`）兩邊都用同一套
機制、對稱實作，不是只做了 Mac 那邊。

**驗證結果**:兩個 TFM 都 0 error；Windows 警告 135→151（+16，全部是新增
的 `PushTransform`/`PopTransform` 與 `WinFormsMouseEvent` 內 `GlobalViewport`
呼叫點的預期 `CA1416` 警告，不是回歸訊號）；非 Windows 警告數不變（8）。
`dotnet run` 再次確認 0 crash、無例外堆疊。畫面實際縮放/置中效果是否正確
（尤其是把視窗拖成很寬或很窄時的留白表現）一樣需要你在自己機器上肉眼
確認。
