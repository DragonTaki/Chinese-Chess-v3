# 跨平台現況

這台開發機是 macOS,裝了 .NET SDK,但這個專案是 `net9.0-windows` +
WinForms + `System.Drawing.Common`——.NET 6 之後這兩個套件的實際繪圖 API
（`Graphics`／`Pen`／`Brush`／`Font` 等)都限定只能在 Windows 上執行。macOS
上**能編譯、不能執行**。

## 能編譯驗證的方式

```
dotnet build Chinese-Chess-v3.csproj -p:EnableWindowsTargeting=true
```

`EnableWindowsTargeting=true` 讓 SDK 在非 Windows 上也能還原/編譯針對
`net9.0-windows` 的專案(靠的是跨平台發佈的參考組件),但**執行不了**,只能
抓語法/型別錯誤,抓不到畫面或互動上的問題。目前 baseline 是 **0 error、
305 warning**(`CA1416` 平台相容性警告,WinForms 專案本來就會有,不是要修的
東西)。

## 「是不是只卡在 UI?」— 逐檔案查證的結果

不完全是。卡住的是**任何有實際呼叫 GDI+ 繪圖 API 或 WinForms 事件型別**的
檔案,不是以資料夾為界線。已經對照一次完整 build 的 610 條 `CA1416` 警告,
按檔案分類如下:

### 卡在 Windows-only API 的檔案

- **`Engine/UI/` 全部**——渲染層本質上就是 GDI+
- **`Engine/GraphicsUtils/`、`Engine/Styles/`**——畫圖路徑、樣式系統,支援
  上面那層
- **`Engine/Timing/TimerManager.cs`**——不是因為畫圖,是因為包了
  `System.Windows.Forms.Timer`
- **`Engine/Configs/EngineSettings.cs`**——存了 `Font`/`Color` 預設值
- **`StarAnimation/Renderers/`**——渲染層
- **`Game/UI/` 全部**——遊戲端的畫面、Handler、樣式
- **`Game/Core/Pieces/PieceSettings.cs`**——存了棋子的 `Font`/`Brush`/`Color`
  常數。**這個放錯資料夾**：內容是視覺設定,不是規則,放在 `Core/`(規則層)
  底下違反 `ARCHITECTURE.md` 說的「`Game/Core` 不該碰畫面相關的東西」。
  應該搬到 `Game/UI/` 底下,但這次先只記錄,不動(移動檔案影響範圍要跟你
  確認)。
- **`Launcher/` 全部**——WinForms 進入點

### 已經完全乾淨、今天就能在 Mac 上單獨 build/測試的部分

- `Engine/Physics/`(這次已完整整理過)
- `Engine/Mathematics/`
- `Engine/Randomization/`
- `Engine/Logging/`(修完 `AppLogger` 之後,現在也不依賴 Game 了)
- `Engine/Globals/`、`Engine/Geometry/`
- `Engine/Network/`(C# client 端的封包/連線邏輯,不含畫面)
- **`Game/Core/` 幾乎全部**(`Boards/`、`Movements/`、`Pieces/` 除了
  `PieceSettings.cs`、`Players/`、`Rules.cs`、`GameManager.cs`)——也就是說,
  **中國象棋的規則引擎本體，現在就已經是平台無關的**，不需要任何改動。

## 換框架不在考慮範圍內

自製 UI/引擎本身就是這個專案的目的——「從底層自己建立東西」是重點,不是
可以被最佳化掉的實作細節。所以「換成 Avalonia 之類的跨平台框架」這個選項
**已經排除**,不管它理論上能不能解決 Mac 執行的問題。

## 實際可行的兩條路

1. **維持現狀**：純邏輯模組(上面「已經乾淨」那份清單)本來就能在 Mac 上
   build/寫單元測試,不用碰任何 UI 相關程式碼。
2. **把純邏輯拆成獨立 class library**：讓 `Engine/Physics`、
   `Engine/Mathematics`、`Game/Core` 等能被單獨引用、單獨測試,不用連著整個
   WinForms 專案一起編譯。工程量中等,不影響任何視覺行為,UI/引擎本身完全
   不動。

目前討論還在確認要走哪條路,這份文件只記錄查證結果,還沒有對應的
`PLAN.md` 項目。
