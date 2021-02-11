# Orimath

**注意:このドキュメントは、未来のリリースについて記述しています。**

* [概要](#概要)
* [動作環境](#動作環境)
* [画面の説明](#画面の説明)

【操作方法】

* [折り線ツール](#-折り線ツール)
  * [点や線の選択(左クリック)](#点や線の選択左クリック)
  * [要素から要素にドラッグして折線をつける](#要素から要素にドラッグして折線をつける)
  * [折り返す (Shiftキー)](#折り返す-shiftキー)
  * [手前の紙だけ折る (Ctrlキー)](#手前の紙だけ折る-ctrlキー)
  * [手前の紙だけ折り返す (Ctrlキー + Shiftキー)](#手前の紙だけ折り返す-ctrlキー--shiftキー)
  * [2つの要素を重ねるように折る (左ドラッグ)](#2つの要素を重ねるように折る-左ドラッグ)
  * [2つの要素を通るように折る (右ドラッグ)](#2つの要素を通るように折る-右ドラッグ)
* [計測ツール](#-計測ツール)
* [コマンド](#コマンド)
* [その他のメニュー](#その他のメニュー)

## 概要

| 項目 | 内容 |
| --- | --- |
| 名前 | Orimath |
| 読み方 | おります |
| バージョン | 0.5-beta.0 |
| 種別 | 折り紙シミュレータ |
| 制作者 | 豊穣ミノリ |

Orimath は数学に特化した折り紙シミュレータです。実際の折り紙と同じようにして平面上で紙を折ることができ、紙上の点や直線を数値で確認することができます。

展開図・折り図制作を主目的としたソフトではありませんが、将来的に便利な機能を搭載する予定があります。

### β版補足

現在公開されているバージョンは **β版** です。
β版ではファイルを保存・出力することができません。

本リリース時には、プロジェクトの保存と読み込み、png・svg形式の出力、他ソフト連携形式での出力に対応する予定です。

## 動作環境

| 項目 | 内容 |
| --- | --- |
| OS | Windows10 |
| フレームワーク | .NET5 |
| ファイルサイズ | 約5.4MB |
| 消費メモリ | 約60MB |
| CPU | 指定なし |

# 画面の説明

![画面の説明](https://github.com/mino-ri/Orimath/blob/master/Documents/Images/view2.png?raw=true)

### ① 折り紙ビュー

メインの作業領域です。ここをクリックやドラッグで操作することで、折り紙を編集します。

### ② 展開図

現在の折り紙の展開図が表示されます。

### ③ 計測ビュー

折り紙ビューで選択している点や直線の数学的情報が表示されます。

### ④ ツール

ツールを切り替えます。

### ⑤ メニューとコマンド

「元に戻す」「裏返す」などの操作を行うために使います。

# 操作方法

## ![icon](https://github.com/mino-ri/Orimath/blob/master/Plugins/Orimath.Basics/Icons/fold.png?raw=true) 折り線ツール

![view](https://github.com/mino-ri/Orimath/blob/master/Documents/Images/view.png?raw=true)

マウスの左右ボタンと、Ctrl・Shift・Altキーの組み合わせで操作します。折り線をつけたり、紙を折り返すことができます。

基本的な操作は以下の通りです。

| 操作 | 結果 |
| --- | --- |
| クリック | 点や線を選択 |
| 左ドラッグ | 2つの要素を重ねるように折る |
| 右ドラッグ | 2つの要素を通るように折る |
| Ctrlキー | 一番手前の紙だけを折る |
| Shiftキー | (折り線をつけるのではなく)折り返す |
| Altキー | 点や線を無視して自由に折る |

### 点や線の選択(左クリック)

紙のカドや辺、折線などをクリックすると、その点や線が **選択された状態** になります。選択された要素は左にある **計測ビュー** で数学的構成が確認できるほか、左ドラッグによる折り方の挙動にも影響します。

### 要素から要素にドラッグして折線をつける

![view](https://github.com/mino-ri/Orimath/blob/master/Documents/Images/fold1.png?raw=true)

紙のカドや辺、折線などにカーソルを合わせてマウスの左ボタンを押し、押したままカーソルを動かすと、図のように点線と緑の矢印が表示されます。

青い点線が表示されている状態でマウスボタンを離すと、点線があった部分に折線がつきます。

### 折り返す (Shiftキー)

![view](https://github.com/mino-ri/Orimath/blob/master/Documents/Images/fold3.png?raw=true)

ドラッグ中にShiftキーを押すと、図のように片方向の青い矢印が表示されます。Shiftキーを押したままマウスボタンを離すと、矢印の向きに紙を折り返します。
折り返す向きはドラッグする方向と同じになります。

### 手前の紙だけ折る (Ctrlキー)

![view](https://github.com/mino-ri/Orimath/blob/master/Documents/Images/fold2.png?raw=true)

ドラッグ中にCtrlを押すと、可能な限り手前にある紙だけを折ることができます。

### 手前の紙だけ折り返す (Ctrlキー + Shiftキー)

![view](https://github.com/mino-ri/Orimath/blob/master/Documents/Images/fold4.png?raw=true)

Shiftキー(折り返す)とCtrlキー(手前の紙だけ折る)を組み合わせることで、手前の紙だけを折り返すことができます。

### 2つの要素を重ねるように折る (左ドラッグ)

左ボタンによるドラッグと右ボタンによるドラッグでは折り方が変わります。左ドラッグでは、ドラッグの開始地点と終了地点を重ね合わせるような折り方になります。

#### 点と点を重ねるように折る

![view](https://github.com/mino-ri/Orimath/blob/master/Documents/Images/axiom2.png?raw=true)

点から点に左ドラッグすると、2つの点を重ねるように折ります。

#### 線と線を重ねるように折る

![view](https://github.com/mino-ri/Orimath/blob/master/Documents/Images/axiom3.png?raw=true)

線から線に左ドラッグすると、2つの線を重ねるように折ります。

複数通りの折り方がある場合、図のように他の候補がグレーの点線で表示されます。ドラッグした位置に最も近い重ね方をするような折り方が候補の中から選択されます。

#### 点と線を重ねるように折る

![view](https://github.com/mino-ri/Orimath/blob/master/Documents/Images/axiom8.png?raw=true)

点から線、または線から点にドラッグすると、それらを重ねるように折ります。この折り方にはバリエーションがあり、ドラッグ前に点や線を左クリックして選択しておくことで挙動が変わります。

---

![view](https://github.com/mino-ri/Orimath/blob/master/Documents/Images/axiom5.png?raw=true)

事前に点を選択しておくと、 **その点を通るように** 折ります。

---

![view](https://github.com/mino-ri/Orimath/blob/master/Documents/Images/axiom7.png?raw=true)

事前に線を選択しておくと、 **その線に垂直になるように** 折ります。

---

![view](https://github.com/mino-ri/Orimath/blob/master/Documents/Images/axiom6.png?raw=true)

Shiftキーを押しながらクリックすることで、点と線を1個ずつ選択することができます。この場合、「選択した点と線」「ドラッグしている点と線」がそれぞれ重なるように折ります。

(理論上可能な折り方なので用意していますが、実際にはあまり使いません)

### 2つの要素を通るように折る (右ドラッグ)

右ドラッグによる折り方は、2つの要素を通るような折り方になります。折り紙よりも作図ツールに近い操作です。

#### 2つの点を通るように折る

![view](https://github.com/mino-ri/Orimath/blob/master/Documents/Images/axiom1.png?raw=true)

図の赤丸で囲った部分を開始地点・終了地点としてドラッグしています。

#### 線に直線で、点を通るように折る

![view](https://github.com/mino-ri/Orimath/blob/master/Documents/Images/axiom4.png?raw=true)

点から線、または線から点に右ドラッグすると、線に垂直で点を通るような折り方になります。

## ![icon](https://github.com/mino-ri/Orimath/blob/master/Plugins/Orimath.Basics/Icons/measure.png?raw=true) 計測ツール

![view](https://github.com/mino-ri/Orimath/blob/master/Documents/Images/measure.png?raw=true)

折り紙上の点や線をドラッグ操作で繋ぐことで、それらの距離や交点などを求めることができます。

## コマンド

画面上部に並んでいるボタンをクリックすることで、折り返しツールでは表現できない操作を行うことができます。

### ![icon](https://github.com/mino-ri/Orimath/blob/master/Plugins/Orimath.Basics/Icons/undo.png?raw=true) 元に戻す (Ctrl+Z)

直前に行った操作を元に戻します。

### ![icon](https://github.com/mino-ri/Orimath/blob/master/Plugins/Orimath.Basics/Icons/redo.png?raw=true) やり直し (Ctrl+Y)

「元に戻す」で戻した内容を戻します。

### ![icon](https://github.com/mino-ri/Orimath/blob/master/Plugins/Orimath.Basics/Icons/rotate_l.png?raw=true) 左に90°回転 (Ctrl+←)

紙全体を左に90°回転させます。

### ![icon](https://github.com/mino-ri/Orimath/blob/master/Plugins/Orimath.Basics/Icons/rotate_r.png?raw=true) 右に90°回転 (Ctrl+→)

紙全体を右に90°回転させます。

### ![icon](https://github.com/mino-ri/Orimath/blob/master/Plugins/Orimath.Basics/Icons/turn_v.png?raw=true) 縦に裏返す (Ctrl+↑)

紙全体を縦に裏返します。

### ![icon](https://github.com/mino-ri/Orimath/blob/master/Plugins/Orimath.Basics/Icons/turn_h.png?raw=true) 横に裏返す (Ctrl+↓)

紙全体を横に裏返します。

### ![icon](https://github.com/mino-ri/Orimath/blob/master/Plugins/Orimath.Basics/Icons/open_all.png?raw=true) すべて開く (Ctrl+E)

折り返してある部分を全て開きます。開いたところには折線が残ります。また、紙を回転させている場合は初期の向きに戻ります。

### ![icon](https://github.com/mino-ri/Orimath/blob/master/Plugins/Orimath.Basics/Icons/new_paper.png?raw=true) 新しい紙 (Ctrl+N)

現在の紙を破棄して、新しい紙を作成します。

このコマンドを使うと、折り紙を **正方形・長方形・正多角形** に変更することができます。

### ![icon](https://github.com/mino-ri/Orimath/blob/master/Plugins/Orimath.Basics/Icons/delete.png?raw=true) すべて削除 (Ctrl+Delete)

全ての折り返しと折線を削除し、まっさらな紙に戻します。

## その他のメニュー

### 設定-環境設定

紙の表示サイズを変更できます。

### 設定-プラグインの設定

各機能の有効/無効を切り替えることができます。基本的には触らないで下さい。