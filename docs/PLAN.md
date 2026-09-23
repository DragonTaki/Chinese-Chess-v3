# 修改規劃

順序原則：**先把地基打好，連線功能跟新功能最後才做**。分層依據見
[`ARCHITECTURE.md`](ARCHITECTURE.md)：先整理 Engine（最底層），再整理 Game
（含它自己的 Rules 底層），再整理 StarAnimation（背景，也是上層但優先度
比 Game 低），最後才輪到 Network（client 端連線）。

另外有一個獨立的大工程——把 GDI+／WinForms 包成可替換的轉換層，之後才有
辦法換一套跨平台後端而不動到自製 UI 系統本身（自製 UI 系統本身不換，見
`CLAUDE.md`）。設計跟分階段規劃見
[`PLATFORM-ABSTRACTION.md`](PLATFORM-ABSTRACTION.md)，還沒排進下面的階段
順序，量體太大，先獨立列出來，等確認要不要做、從哪個子階段開始再排入。

## 階段 1 — Engine（最底層，最先整理，要求完善）

- [x] `Engine/UI/Core/Renderers/UIContainerRenderer.cs:48` 的 TODO——已加上
  可選的 `IBoxDrawStyle Style` 掛勾點，沒指定 `Style` 的容器（目前
  `UISidebar`、`UIScrollContainer` 都是）行為不變。
- [x] `Engine/SharedLib.csproj`／`StarAnimation.csproj` 沒作用的問題——已刪除
  這兩個空轉的 `.csproj`，改用單一專案，跟刪除前的實際編譯行為相同。真的
  拆成獨立 class library（強制「Engine 不依賴任何人」）故意先不做：Engine
  裡有 5 處 `internal`，拆開後要逐一確認可見度，這只能在 Windows 上
  build＋跑起來驗證，等有 Windows 環境再做，見 STATUS.md「Build 設定」。
- [x] 5 處架構違規（`AppLogger`、`Physics2D`、`UIOverlayMask`、
  `DialogManager`、`UIMenuRenderer` 對 `Game.*` 的依賴）——全部修完，`Engine/`
  對 `Game.*` 的引用歸零。
- [x] `Engine/Physics/` 完整整理——記憶體洩漏、死碼分支、null 檢查、
  **`SmoothUpdate()` 缺 deltaTime 造成幀率相依的模擬**都修了，細節見
  STATUS.md「2D 物理引擎」。常數重新調校（`SpringK`／`Damping`／
  `AccelerationLerpFactor`）跟 `Acceleration.Current` 的 Lerp 平滑本身的
  幀率相依問題，故意留給你在 Windows 上處理。
- Engine 完善之後，才有資格說「Game／StarAnimation／Network 都能放心依賴
  它」——這階段做完前，不要往上動 Game 或 StarAnimation 的架構性修改。

## 階段 2 — Game（含它自己的 Rules 底層）

- **HalfCenter（8×4）與 HalfCross（9×5）走法規則未實作**——
  `Game/Core/Pieces/PieceTypes/*.cs` 每個棋子的
  `GetLegalMovesHalfCenter` / `GetLegalMovesHalfCross` 都是空的。這是
  Game/Core 規則底層裡最大的洞，優先處理。
- **決定 `Game/Core/Rules.cs` 裡沒被用到的規則旗標**要不要留（暗棋、連吃、
  車衝、馬走斜、包必須跳吃、可吃己方棋子、自殺移動）。要嘛接進
  `PieceTypes/*.cs` 的走法判斷，要嘛刪掉。
- **棋譜／回放系統**：把 `GameManager.cs` 目前純文字 log 的紀錄方式，換成
  結構化的移動清單（標準記譜法），才能支援悔棋跟回放。
- 確認 `Game/Core/` 沒有 `using` 到 `Engine.UI` 或 `Game.UI`——規則邏輯要能
  脫離畫面獨立測試（見 ARCHITECTURE.md 的依賴方向規則）。
- 多陣營系統（魏／蜀／吳、旗／火等特殊棋子）——這階段裡優先度最低，純加法
  功能，等前面幾項做完再考慮。

## 階段 3 — StarAnimation（背景，上層但優先度低於 Game）

- 刪掉 `StarAnimation/DEPRECATED/`——已排除在 build 之外，零引用，純粹
  佔位。
- 目前沒發現其他功能性缺口；這階段主要是清理，等階段 1、2 做完再處理。

## 階段 4（最後）— Network（client 端連線）＋ 跟 Server 對齊

連線功能放在最後，等 Engine／Game／StarAnimation 這三塊地基都整理完再動：

1. 把 `Engine/Network/` 搬到跟 `Game/`、`StarAnimation/` 同層的頂層
   `Network/` 資料夾，讓實體結構符合 ARCHITECTURE.md 定義的邏輯分層
   （Network 不屬於 Engine 地基層）。
2. 決定連線層的去留：目前 `GameClient.cs`／`NetworkManager.cs` 兩套並存，
   兩者都沒被實際建立過實例。留 `NetworkManager.cs`（較完整，已有心跳機制
   跟 `AuthManager` 整合），刪掉 `GameClient.cs`。
3. 接上去之前先修好 `NetworkManager.StartListening` 對每個非心跳封包
   重複呼叫 `OnPacketReceived` 兩次的 bug。
4. 從一個真正的進入點（例如 Game 的多人連線選單）去建立 `NetworkManager`
   實例，讓呼叫方向維持「Game → Network」，Network 不反過來驅動 Game。
5. 等有真正的登入畫面後，拿掉 `AuthManager.cs` 裡寫死的測試帳密。

這份規劃要搭配
[`Chinese-Chess-v3-Server/docs/PLAN.md`](https://github.com/DragonTaki/Chinese-Chess-v3-Server/blob/main/docs/PLAN.md)
一起看：Server 是完全獨立的系統（見 ARCHITECTURE.md），不是 client 端
Network 模組的上下層，而是協定的另一端。伺服器端目前完全沒有房間／對局的
封包處理邏輯，所以就算階段 4 做完，也還沒辦法真的玩到一場多人對局——但
那是 Server 那邊的規劃範圍，不影響這裡的階段順序。
