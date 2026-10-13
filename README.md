# KINEMA 播放器

取代 MPC-BE + K-Lite 的 Windows 影片播放器。

- 單一安裝檔：安裝時關聯影片副檔名，在檔案總管雙擊影片就播放
- 內建全部解碼器（FFmpeg / libmpv），不需要 K-Lite、不註冊系統濾鏡
- 補幀一鍵開關（I 鍵 / 播放列按鈕）：內建 VapourSynth + MVTools 動態補償，載入失敗會自動改用平滑模式
- NVIDIA RTX Video Super Resolution / RTX Video HDR（Ctrl+R / Ctrl+H）
- 字幕調整：字型、顏色、大小、位置、外框、時間偏移、外掛字幕
- 快捷鍵接近 MPC-BE，右鍵選單、記住播放位置、單一視窗開啟

## 取得安裝檔（不用寫程式）

1. 到 https://github.com 註冊免費帳號。
2. 右上角「+」→「New repository」，名稱填 kinema，選 Public，按 Create。
3. 在新的 repository 頁面按「uploading an existing file」，把本資料夾**裡面的全部內容**
   （含 .github 資料夾）拖進去，按「Commit changes」。
4. 點上方「Actions」分頁，若出現提示就按啟用。編譯會自動開始，約 5–10 分鐘。
5. 完成後點右側「Releases」，下載 **KINEMA-Setup.exe** 執行安裝。
6. 安裝最後一頁勾選「開啟 Windows 預設應用程式」，把 KINEMA 設為影片的預設程式。
   （Windows 10/11 規定這一步必須由使用者自己確認，任何播放器都一樣。）

之後要更新：在 Actions 分頁選「Build KINEMA」→「Run workflow」，會抓最新版的 mpv 核心重新打包。

> Windows 可能顯示 SmartScreen 警告（因為安裝檔沒有付費數位簽章），按「其他資訊」→「仍要執行」。

## 快捷鍵

| 按鍵 | 功能 |
|---|---|
| 空白 / 單擊 | 播放 / 暫停 |
| 雙擊 / Enter / Alt+Enter | 全螢幕 |
| 右鍵 | 選單（音軌、字幕軌、補幀、設定） |
| ← / → | 倒退 / 快轉 5 秒（Ctrl：30 秒） |
| ↑ / ↓ / 滾輪 | 音量 |
| PgUp / PgDn | 上一個 / 下一個檔案 |
| I | 補幀 開 / 關 |
| Ctrl+R / Ctrl+H | RTX VSR / RTX HDR |
| Ctrl+S | 字幕調整 |
| Ctrl+↑ / Ctrl+↓ | 字幕大小 |
| Shift+↑ / Shift+↓ | 字幕位置 |
| Z / X | 字幕提前 / 延後 0.1 秒 |
| A / S | 切換音軌 / 字幕 |
| Tab | 統計資訊（可確認輸出幀率） |
| Ctrl+O | 開啟檔案 |

## 進階

設定存在 %APPDATA%\KINEMA。可以在那裡放自己的 mpv.conf 覆寫任何 mpv 選項。

## 結構

- src/Kinema — C# .NET 8 WinForms 程式（嵌入 libmpv）
- installer/kinema.iss — Inno Setup 安裝檔（含副檔名關聯）
- .github/workflows/build.yml — 自動下載 libmpv、VapourSynth、MVTools 並打包
