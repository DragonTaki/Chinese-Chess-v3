# 修改規劃

依據 `STATUS.md` 目前的缺口排出的優先順序。

1. **決定連線層的去留。** 目前存在兩套互不相干的實作（`GameClient.cs`、
   `NetworkManager.cs`），兩者都沒接到正在跑的 App 上。挑一個留下——
   `NetworkManager.cs` 相對完整（已經有心跳機制跟 `AuthManager` 整合）——
   刪掉另一個，然後真的從一個進入點（例如多人連線選單）去建立實例。接上去
   之前要先修好 `NetworkManager.StartListening` 的重複呼叫 bug。等有真正的
   登入畫面後，再拿掉 `AuthManager.cs` 裡寫死的測試帳密。
2. **實作 HalfCenter（8×4）與 HalfCross（9×5）的走法規則。** 每個棋子的
   `GetLegalMovesHalfCenter` / `GetLegalMovesHalfCross` 都還沒實作——這卡住
   了 README 目前宣稱已支援的明棋／暗棋／三國模式。
3. **決定那些沒被用到的規則變化旗標**（暗棋、連吃、車衝、馬走斜、包必須跳吃、
   可吃己方棋子、自殺移動）要不要留。它們在 `Rules.cs` 裡定義了，但沒有任何
   地方會讀取——要嘛把它們接進走法合法性判斷裡，要嘛就刪掉，別讓設定檔
   看起來有選項但其實什麼都不會發生。
4. **棋譜／回放系統。** 把目前的純文字 log 換成結構化的移動清單（標準記譜法，
   例如「俥一平三」），才能支援悔棋跟回放——README 自己也已經承認這是
   已知的缺口。
5. **刪掉死碼**：`DEPRECATED/` 與 `StarAnimation/DEPRECATED/`——已經被排除在
   build 之外，也沒有任何引用，純粹是佔位的雜物。
6. **解決專案拆分的問題。** 要嘛把 `Engine/SharedLib.csproj` 跟
   `StarAnimation/StarAnimation.csproj` 真的併回主專案（順便移除這兩個目前
   沒作用的 `.csproj`），要嘛就真的把它們拆成獨立的 class library 並引用。
   現在這兩個檔案存在但什麼都沒編譯出來。
7. **多陣營系統**（魏／蜀／吳、額外棋子）——優先度最低，純加法功能，沒有
   其他東西依賴它。

## 連線層搞定之前先不要做的事

任何依賴連線層的功能（遊戲內聊天、配對、即時對手同步）都要等第 1 項做完
再說——目前 client 端根本沒有東西可以連到伺服器。

這份規劃要搭配
[`Chinese-Chess-v3-Server/docs/PLAN.md`](https://github.com/DragonTaki/Chinese-Chess-v3-Server/blob/main/docs/PLAN.md)
一起看：伺服器端目前完全沒有房間／對局的封包處理邏輯，所以就算把 client
端的連線層接上去，也還沒辦法真的玩到一場多人對局。
