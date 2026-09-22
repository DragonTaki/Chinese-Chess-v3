# 架構分層

跟公司產品一樣，用「等級層次 + submodule」的方式劃分，而不是單純照資料夾分。
目的是先確認每個模組該不該依賴誰，再回頭排開發／整理的優先順序（見
[`PLAN.md`](PLAN.md)）。

## 分層總覽

```
Layer 0（最底層，地基）
  Engine/                自製引擎：畫面（UI 系統）與運算（2D 物理）
    ├─ UI/               元件樹式 UI 系統：渲染、事件路由、生命週期
    ├─ Physics/           2D 物理引擎：位置／速度／加速度、彈簧、阻力
    ├─ Mathematics/       Vector2F、數學函數
    ├─ Geometry/          LayoutF 等版面計算
    ├─ GraphicsUtils/     繪圖路徑輔助
    ├─ Styles/            UI 樣式系統
    ├─ Timing/            GlobalTime、TimerManager
    ├─ Randomization/     隨機數系統
    ├─ Logging/           日誌系統
    └─ Configs/, Globals/ 全域設定與視窗狀態

Layer 1（建立在 Engine 之上，三個互相獨立的模組，只往下呼叫 Engine）
  Game/                  中國象棋本體
    ├─ Core/              ★ 遊戲自己的規則底層（不依賴 Engine/UI）
    │   ├─ Boards/         Board.cs、BoardConstants.cs、BoardConfigLoader.cs
    │   ├─ Movements/       MoveDirections、MoveMatrix、MovePatterns
    │   ├─ Pieces/          Piece.cs 及各棋種 PieceTypes/*.cs
    │   ├─ Players/         Player、PlayerSide、PlayerTimer
    │   ├─ Rules.cs         規則旗標定義
    │   └─ GameManager.cs   遊戲流程控制
    └─ UI/                 建立在 Engine/UI 之上的遊戲畫面（選單、棋盤、側邊欄）

  StarAnimation/         動態星空背景模組
    ├─ Core/Effect/        效果定義（閃爍、紅移、黑洞）
    ├─ Controllers/         星體／畫面控制器
    └─ Renderers/           自己的渲染層（不是走 Engine/UI，是獨立渲染管線）
    只用到 Engine/Physics 做漂移／回彈模擬，跟 Game 完全獨立、互不依賴。

  Network/（client 端）  ★ 目前實體上放在 Engine/Network/，但邏輯上不屬於
                         Engine 這個地基層——它是 Game 才會用到的獨立模組，
                         Engine 本身不依賴它，也不該依賴它。
    現況：GameClient.cs / NetworkManager.cs（詳見 STATUS.md）。
    呼叫方向：Game → Network（Game 決定要不要連線，Network 不會反過來
    驅動 Game）。

Layer 2（完全獨立的系統，不算在這個 repo 的分層裡）
  Chinese-Chess-v3-Server（另一個 repo）
    Network 的另一端。跟 client 端的 Network 模組是「協定的兩端」關係，
    不是「上下層」關係——Server 有自己的 Layer 0（DB、auth）跟 Layer 1
    （房間／對局邏輯），細節見該 repo 的 STATUS.md / PLAN.md。
```

## 依賴方向規則

- **Engine 不依賴任何人**——不管是 Game、StarAnimation 還是 Network，
  Engine 都不應該 `using` 到它們的命名空間。
- **Game、StarAnimation、Network 三者互相獨立**，彼此不呼叫彼此，只往下
  呼叫 Engine。目前 Network 物理上放在 `Engine/Network/` 底下容易讓人誤會
  它是 Engine 的一部分——等處理到 Network（見 PLAN.md 最後階段）時，
  應該連同功能修改一起把它搬到跟 `Game/`、`StarAnimation/` 同層的
  `Network/` 資料夾，讓實體結構跟邏輯分層對得上。
- **Game/Core（規則底層）跟 Game/UI 也要分清楚**：`Core/` 不應該
  `using` 任何 `Engine.UI` 或 `Game.UI` 的東西，規則判斷要能脫離畫面
  單獨測試。
- **Server 是完全獨立的系統**，跟 client 端的 Network 模組不是同一層——
  它是通訊協定的另一端，不是 client 的下層或上層。
