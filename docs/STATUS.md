# 實作狀態

以實際程式碼為準（不是 `README.md` 的功能敘述）。
狀態標記：**已完成** / **部分完成** / **僅有骨架** / **未實作**。

## 中國象棋（`Game/`）

- **大盤（9×10，經典）** — 已完成。每個棋子（`Elephant.cs`、`Horse.cs` 等）都
  有真正的 `IsValidMoveFull` / `GetLegalMovesFull` 邏輯，並會實際讀取
  `board.GameRules` 的旗標（`CanGeneralSeeGeneral`、`CanElephantEyeBlockd`、
  `CanHorseLegHobbled` 都真的有被檢查）。
- **HalfCenter（8×4，明棋／暗棋）與 HalfCross（9×5，三國模式）** — 僅有骨架。
  盤面尺寸已在 `BoardConstants.cs` 定義，但每個棋子的
  `GetLegalMovesHalfCenter` / `GetLegalMovesHalfCross` 都還沒實作
  （`// Not implement yet`），完全沒有走法邏輯。
- **規則變化旗標**（`IsHiddenChess`、`CanCaptureHiddenPiece`、
  `IsAllowChainCapture`、`CanChariotRush`、`IsHorseMoveDiagonally`、
  `IsCannonMustJumpToCapture`、`CanCaptureOwnPiece`、`CanSuiside`）— 未實作。
  在 `Rules.cs` 裡定義了，但整個程式碼裡沒有任何地方讀取。
- **多陣營系統**（魏／蜀／吳、旗／火等特殊棋子）— 未實作。`PieceType` 只有
  標準的 7 種棋子加 `Shadow`；`HalfCrossTeamSetup` 也只設定了紅／黑兩方，
  完全沒有陣營概念。
- **棋譜／回放系統**（標準記譜法）— 未實作。目前的「紀錄」只是純文字 log
  （`GameManager.cs` 約 193-247 行，透過 `UILoggerBox`），不是結構化的移動
  清單，沒有悔棋，也沒有回放。
- **比賽計時器** — 已完成。`PlayerTimer.cs` — 正數／倒數計時、局時與步時、
  無限模式，事件驅動。

## 2D 物理引擎（`Engine/Physics/`）

已完整整理（bug、記憶體洩漏、正確性都處理過，見 commit history）：

- **已修**：`PhysicsRegistry` 用強引用 `HashSet` + finalizer 做 unregister 造成的記憶體洩漏（改成 `WeakReference`）。
- **已修**：阻尼（damping）邏輯因為兩個互斥條件疊在一起，永遠不會執行的死碼。
- **已修**：`EnforceBoundaries()` 對 `Boundary` 本身缺 null 檢查會炸的問題。
- **已修（較大的一項）**：`SmoothUpdate()` 整個積分過程原本完全沒有乘上
  deltaTime——`Velocity.Current += Acceleration.Current`、
  `Position.Current += Velocity.Current` 都是直接累加，等於模擬速度綁死在
  「多久呼叫一次 `SmoothUpdate()`」，不是真實時間。專案裡
  `TimerManager.DeltaTimeInSeconds` 早就算好且被 `StarAnimation` 用了，只有
  `Physics2D` 自己沒用到。已改成 `GlobalTime.Timer.DeltaTimeInSeconds` 正確
  縮放（semi-implicit Euler，業界標準做法）。
- **驗證方式的限制**：以上都只做了 `dotnet build` 編譯驗證，這台機器是
  macOS，WinForms 無法實際執行，沒辦法用眼睛確認手感。
- **待辦，故意先不動**：
  - `SpringK`／`Damping`／`AccelerationLerpFactor` 這幾個常數，很可能是作者
    在「沒有 deltaTime」的舊行為下肉眼調出來的。補上 deltaTime 後，同一組
    數值的實際意義變了，視覺上的手感（彈簧軟硬、移動速度）大概率會跟以前
    不一樣，需要在 Windows 上重新試、重新調——這台機器做不到，先留給你。
  - `Acceleration.Current = Vector2F.Lerp(Acceleration.Current, Acceleration.Target, AccelerationLerpFactor)`
    這行本身也是每次呼叫用固定係數做插值，同樣有幀率相依的問題（標準修法是
    用 `1 - MathF.Pow(1 - lerpFactor, deltaTime * 60)` 這類指數衰減公式，而不
    是單純乘 deltaTime）。這個沒有一起修，因為修法本身也會改變手感，跟上面
    的常數問題是同一類「需要在 Windows 上重新調」的項目，一起留著。

## 連線系統（`Engine/Network/`）

做出來了，但完全沒接到正在跑的 App 上。

- 同時存在兩套互不相干的實作：`GameClient.cs`（很簡略）與
  `NetworkManager.cs`（有心跳機制與 `AuthManager` 整合）。**兩者都從未被
  實際建立過實例**——沒有任何選單或按鈕會去建立連線。
- `GameClient.cs:41` — 封包處理是空的：
  `// TODO: 根據 packet.Type 更新棋局或倒計時`。
- `AuthManager.cs` 寫死了測試帳密（`"1@test.com"` / `"1"`）——目前沒被
  使用所以無害，但真的要接上時必須先拿掉。
- **Bug**：`NetworkManager.StartListening` 對每個非心跳封包都會呼叫兩次
  `OnPacketReceived`（deserialize 之後一次，心跳檢查之後又一次）。

## 平台轉換層（`Engine/Platform/`）

已完成:`IGraphics`／`IBrush`／`IPen`／`IFont`／`IGraphicsPath` 等介面 +
WinForms 後端，已經完整接通到 `Engine/UI`、`Engine/Styles`、`Game/UI`、
`StarAnimation` 整條繪圖鏈——不再有任何 Renderer 或樣式檔案直接碰
`System.Drawing` 的 GDI+ 型別。細節、涵蓋清單、剩下的滑鼠輸入/視窗部分見
[`PLATFORM-ABSTRACTION.md`](PLATFORM-ABSTRACTION.md)。

## 死碼

- `DEPRECATED/`（頂層）與 `StarAnimation/DEPRECATED/` — 已透過 `.csproj`
  的 glob 規則排除在 build 之外（`DEPRECATED\**\*.cs`），全專案零引用。
  可以安全刪除。
- `Engine/Configs/EngineSettings.cs` — 整個檔案（`DefaultScrollTextFont` 等
  4 個 `ScrollTextBox` 預設值）在全專案零引用，確認過。

## Build 設定

已解決：`Engine/SharedLib.csproj` 與 `StarAnimation/StarAnimation.csproj` 這兩個
沒被任何地方引用、自己什麼都沒編譯出來的 `.csproj` 已經刪除，`.csproj` 裡
指向 `StarAnimation.csproj` 的註解掉的 `ProjectReference` 也一併清掉。現在是
單一專案（`Chinese-Chess-v3.csproj`），`Engine/**/*.cs` 與
`StarAnimation/**/*.cs` 一樣透過 SDK-style 預設 globbing 掃進同一個 exe，
跟刪除前的實際編譯行為完全相同。

真的把 Engine 拆成獨立 class library（用 `ProjectReference`、在編譯期強制
「Engine 不依賴任何人」）目前故意不做：Engine 裡有 5 個檔案用了 `internal`
存取修飾詞，拆成獨立組件後這些型別對 `Game`/`StarAnimation` 就會不可見，
必須逐一確認要不要開放（改 `public` 或用 `InternalsVisibleTo`）——這件事
只能在 Windows 上實際 build＋跑起來驗證，這台機器做不到，所以先不動，留在
[`PLAN.md`](PLAN.md) 當作之後有 Windows 環境時的項目。

## 程式碼裡的 TODO

- `Engine/Network/GameClient.cs:41`
- `DEPRECATED/Panels/MainMenuPanel.cs:125`（死碼，不重要）
