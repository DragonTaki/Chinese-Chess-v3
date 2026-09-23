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

建議分階段做,每一階段都要 build 驗證(`dotnet build -p:EnableWindowsTargeting=true`)
維持 0 error、警告數量只減不增(換成介面呼叫後,原本的 `CA1416` 警告會消失,
因為呼叫點不再直接碰 GDI+ 型別,只有 `WinForms/` 資料夾底下的實作檔案還會有):

1. **`IFont`／`IBrush`／`IPen`／字型相關**——用量最大、但邏輯最簡單(大多是
   資料容器,沒有複雜的繪圖狀態機),先做這批建立信心跟驗證介面形狀對不對。
2. **`IGraphics`**——最核心、呼叫最多方法的一個,等 1 做完、介面設計穩定
   後再動,牽連最廣。
3. **`IGraphicsPath`／`IMatrix`／`IRegion`**——只有 `Engine/GraphicsUtils`
   跟少數 `Engine/Styles` 檔案在用,範圍小。
4. **`IWindow`／`IMouseEvent`**——只有 `Launcher/` 兩三個檔案跟輸入處理相關
   的檔案在用,量小,但要等 `IGraphics` 做完才能讓 `MainForm` 真正透過
   介面繪圖。

第 1 步做完之後才決定要不要繼續——這個規模已經超過「整理」的範圍,是一個
獨立的大工程,先做第 1 步看實際情況再往下排。
