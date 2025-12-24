# PAST (ProjectG Automated Shot Tool)
[Language: [日本語](#japanese) / [English](#english)]

<a name="japanese"></a>
メモリ監視を使用した、ProjectGエミュレータサーバー向けの自動ショットツールです。

## 主な機能
- メモリ読み取りによる正確なパワーゲージ監視(現在は800x600x32のみ)
- 指定％での自動スペースキー送信
- スキルコマンド（トマホーク、コブラ等）とスピンの自動入力

## 使い方
1. Visual Studioでビルドします。
2. 管理者権限で実行します。
3. ゲームを起動し、コースに入ってショットゲージが表示されてから「Start」を押します。
4. ％を指定し、入力する特殊ショットやスピンを選び、ゲーム内でゲージを作動させます。
5. 「２」キーを押したままで指定した％で自動的にスペースキーが押されます。
6. 50％を切る前に「１」キーを押したままで選択した特殊ショットやスピンが入力され、その後0％で自動的にスペースキーが押されます。

「１」キー、「２」キーはそれぞれPangYa・Powerの項目を書き換えることで変更できます。
対応キーは「仮想キーコード」で調べて、10進数に変換してください。

----

<a name="english"></a>
An automated shot tool for ProjectG emulator servers using memory monitoring.

## Main Features
- Accurate power gauge monitoring via memory reading (currently only 800x600x32)
- Automatic spacebar presses at specified percentages
- Automatic input of skill commands (Tomahawk, Cobra, etc.) and spins

## Usage
1. Build using Visual Studio.
2. Run with administrator privileges.
3. Launch the game, enter a course, wait for the shot gauge to appear, then press “Start”.
4. Specify the percentage, select the special shot or spin to input, and activate the gauge in-game.
5. Hold down the “2” key to automatically press the spacebar at the specified percentage.
6. Hold down the “1” key before the percentage drops below 50% to input the selected special shot or spin. After reaching 0%, the spacebar will be pressed automatically.

The “1” key and “2” key can be changed by modifying the PangYa and Power entries respectively.
Search for the corresponding key using “virtual key code” and convert it to decimal.
