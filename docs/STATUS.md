# 實作狀態

以實際程式碼為準（不是 `README.md` 的功能敘述）。
狀態標記：**已完成** / **部分完成** / **僅有骨架** / **未實作**。

## 中國象棋（`Game/`）

- **大盤（9×10，經典）** — 已完成。每個棋子（`Elephant.cs`、`Horse.cs` 等）都
  有真正的 `IsValidMoveFull` / `GetLegalMovesFull` 邏輯，並會實際讀取
  `board.GameRules` 的旗標（`CanGeneralSeeGeneral`、`CanElephantEyeBlockd`、
  `CanHorseLegHobbled` 都真的有被檢查）。
  - **已修**：`Elephant.GetLegalMovesFull` 原本呼叫
    `MovePatterns.GetDiagonalLShape`（馬走的 L 型方向表），而不是
    `IsValidMoveFull` 正確使用的 `GetDiagonalTwoStep`（象走的兩步斜線）。
    因為 `IsElephantEyeBlocked` 對輸入座標有防呆檢查
    （`Math.Abs(dx)!=2 || Math.Abs(dy)!=2` 就丟例外），L 型座標必定觸發，
    等於**只要呼叫 `piece.GetLegalMoves()` 選到大盤的象，一定會丟未處理例外
    直接當掉**。目前這個方法在整個專案裡沒有任何呼叫點（見下面「僅有骨架」
    的說明也是同樣狀況），所以還沒被玩家實際觸發過，但只要之後接上「顯示
    可走位置」這種功能就會立刻炸。已修正为呼叫 `GetDiagonalTwoStep`，並寫了
    一次性的 scratch 測試確認不再丟例外。
- **HalfCenter（8×4，明棋／暗棋）** — 走法／吃子規則已實作。所有棋子預設
  都是上下左右走一格；`Rules.CanChariotRush` 開啟時俥／車恢復大盤直線衝殺，
  `Rules.IsHorseMoveDiagonally` 開啟時馬改成斜走一格；包永遠可以直線滑動，
  依 `Rules.IsCannonMustJumpToCapture`（預設開啟）決定吃子要不要跳一顆
  「炮架」。吃子合法性統一由新的 `Piece.CanCaptureAtHalfCenter` 判斷：不能
  吃己方；不能吃明棋裡比自己弱的（依 `Rules.PieceRankings` 排名）；預設
  （`CanCaptureHiddenPiece = false`）完全不能吃暗子。
  - **還沒做**：`CanCaptureHiddenPiece` 開啟後「吃到比自己強的暗子等於同歸
    於盡」（`IsCaptureHiddenPieceStrongerSuiside`）這個雙方棋子都要死的狀態
    變化，只判斷了「能不能嘗試」，實際执行後的雙亡邏輯要在 `Board`/
    `GameManager` 執行移動的地方另外接。
  - **還沒做**：開局隨機洗牌＋翻面設定（暗棋要洗牌，明棋不用）、`GameManager`
    真的依選單選擇建立 `BoardType.HalfCenter` 的 `Board`（目前
    `GameManager.cs:80` 永遠是 `new Board()`，也就是永遠是大盤，
    `UINewGameMenuOptions.cs` 裡「暗棋半盤」「明棋半盤」兩個按鈕目前點了沒
    有實際效果）。
  - `IsValidMoveHalfCenter` 原本（連同 `HalfCross`）完全沒被覆寫，基底類別
    預設回傳 `true` ——**代表這兩種盤面之前任何棋子可以移動到任何位置，
    完全沒有規則限制**，比原本文件寫的「僅有骨架／沒有走法提示」更嚴重。
    HalfCenter 這次已經補上真正的合法性判斷，HalfCross 因為牽涉三方陣營、
    要先跟你確認規則細節，還沒動。
- **HalfCross（9×5，三國模式）** — 僅有骨架，同上，未實作，原因見
  `docs/PLAN.md`。
- **揭棋（大盤變體，`Rules.IsJieqi`）** — 棋子的「移動規則」判斷已實作：
  暗子（`IsFaceUp == false`）的第一步會依照
  `PieceConstants.GetClassicPieceTypeAt(x, y)` 查出的「這格古典佈局上原本
  該放哪種棋子」，暫時借用該棋子類型的大盤規則來判斷合不合法（做法是建立
  一個那個類型的臨時 `Piece` 實例，直接呼叫它未修改過的 `IsValidMoveFull`,
  不去改動 7 個棋子各自的邏輯本身）；一旦翻開（`IsFaceUp == true`）就恢復
  用自己真正的類型判斷。另外照查到的規則實作了「翻開後的仕／相可以無視
  九宮／過河限制，自由走動」這個揭棋特有的規則。已寫 scratch 測試驗證：
  暗子在車的起始位置第一步走直線合法、走馬步不合法；翻開後（假設真身是
  馬）換成走馬步合法、走直線不合法；翻開的相可以過河，一般大盤的相不行。
  - **還沒做**：翻開（`IsFaceUp` 從 false 變 true）這個狀態轉換本身——目前
    只有判斷邏輯，沒有「移動成功後自動翻面」的執行步驟，這要接在
    `Board.MovePiece`／`GameManager` 實際執行移動的地方。
  - **還沒做**：開局洗牌設定（除了雙方將帥維持原位明棋，其餘 14 顆棋子要
    隨機打亂、蓋牌放到自己那一側的起始位置）、`GameManager` 真的依選單
    （`UINewGameMenuOptions.cs` 已經有「揭棋大盤」按鈕）建立
    `Rules.IsJieqi = true` 的對局。
- **規則變化旗標**（`IsHiddenChess`、`CanCaptureHiddenPiece`、
  `IsAllowChainCapture`、`CanCaptureOwnPiece`、`CanSuiside`）— 仍未實作
  （`CanChariotRush`／`IsHorseMoveDiagonally`／`IsCannonMustJumpToCapture`
  這三個這次已經接進 HalfCenter 了，其餘的還是定義了但沒有任何地方讀取）。
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

已完成:`IGraphics`／`IBrush`／`IPen`／`IFont`／`IGraphicsPath`／`IWindow`／
`IMouseEvent` 等介面，已經完整接通到 `Engine/UI`、`Engine/Styles`、
`Game/UI`、`StarAnimation` 整條繪圖與輸入鏈——不再有任何 Renderer、樣式
檔案或輸入處理類別直接碰 `System.Drawing`／`System.Windows.Forms` 的型別。

兩個後端都已建好並驗證過:

- `Engine/Platform/WinForms/`（`Launcher/`）—— GDI+/WinForms，`net9.0-windows`
  build，0 error。
- `Engine/Platform/Skia/`（`Launcher.Cross/`）—— SkiaSharp（繪圖）+
  Silk.NET（視窗/輸入/OpenGL），`net9.0` build，0 error，且在這台 macOS
  機器上 `dotnet run` 起來過（DI／UI 樹初始化成功，選單可操作）。

細節、涵蓋清單、還沒能在這個沙盒環境驗證到的部分（視覺畫面本身）見
[`PLATFORM-ABSTRACTION.md`](PLATFORM-ABSTRACTION.md) 最後兩節。

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
