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
