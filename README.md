[![C#](https://custom-icon-badges.demolab.com/badge/Built%20with-C%23-%23239120.svg?style=flat-square&logo=cshrp&logoColor=white)](https://dotnet.microsoft.com/languages/csharp)

---

## 💡 About This Project

This project is only for private use.

以 C# 編寫的中國象棋桌面用戶端，建立在自製的 UI 系統、自製的 2D 物理引擎，
以及相依注入（[Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/9.0.0)）之上。

包含四個主要部分：自製 UI 系統（元件樹架構，MVVM/MVC 混合）、自製 2D 物理引擎
（位置／速度／加速度、彈簧、阻力、時間步進模擬）、中國象棋本體（多種盤面、規則
變化、比賽計時器），以及建構於物理引擎之上的動態星空背景模組。

配套的多人連線後端伺服器是 Golang 寫的 `Chinese-Chess-v3-Server`。

目前實際完成度與尚未完成的部分請見 [`docs/STATUS.md`](docs/STATUS.md)；
後續修改的優先順序規劃請見 [`docs/PLAN.md`](docs/PLAN.md)；分層架構請見
[`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md)；在非 Windows 機器上開發時的
限制與現況請見 [`docs/CROSS-PLATFORM.md`](docs/CROSS-PLATFORM.md)；把繪圖/
視窗包成可替換轉換層的設計請見
[`docs/PLATFORM-ABSTRACTION.md`](docs/PLATFORM-ABSTRACTION.md)。

---

## 🛡️ About Chinese Chess

Chinese chess is a strategy board game for two players.
It is the most popular board game in East Asia.

---

## 📜 Third-Party Licenses

- 僅供個人學術研究使用，可相互探討，禁止抄襲（包含部分抄襲）
- 禁止搬運、複製或儲存於其他儲存庫中
- 禁止當作作業 Project

> This repository **does not provide any license or redistribution rights**
> for its own code. It is intended for **private use only**.

---

## 👤 About Author

- [Discord](https://discord.gg/GDMSyVt)
- [Twitch](https://bit.ly/DragonTakiTwitch)
- [YouTube](https://bit.ly/DragonTakiYTNew)
- [Twitter](https://twitter.com/MacroDragonTaki)
- [Fur Affinity](https://bit.ly/DragonTakiFA)
- [巴哈姆特](https://bit.ly/DragonTakiBaha)
