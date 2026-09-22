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

## 死碼

- `DEPRECATED/`（頂層）與 `StarAnimation/DEPRECATED/` — 已透過 `.csproj`
  的 glob 規則排除在 build 之外（`DEPRECATED\**\*.cs`），全專案零引用。
  可以安全刪除。

## Build 設定

`.sln` 裡只有 `Chinese-Chess-v3.csproj` 被引用。`Engine/SharedLib.csproj`
與 `StarAnimation/StarAnimation.csproj` 是獨立的專案檔，但沒有被任何地方
引用（`StarAnimation` 的 `ProjectReference` 被註解掉了）。因為主專案用的是
SDK-style 預設 globbing，`Engine/**/*.cs` 與 `StarAnimation/**/*.cs` 還是會
被直接掃進同一個 exe——這兩個額外的 `.csproj` 目前自己什麼都沒編譯出來。

## 程式碼裡的 TODO

- `Engine/UI/Core/Renderers/UIContainerRenderer.cs:48`
- `Engine/Network/GameClient.cs:41`
- `DEPRECATED/Panels/MainMenuPanel.cs:125`（死碼，不重要）
