# 折紙公理

折紙公理には次の7つが存在する。これらは、真の意味で公理ではない。

## 序. 基本情報

### 各要素の表現について

折紙の数学は点と直線で構成される。

#### 点

$(x, y)$ で表される。2次元ユークリッド空間上の1点を表す。

#### 直線
一般式 $ax + by + c = 0$ (a,b,cは定数, $a > 0$ または $b > 0$ )で表される。 折紙公理の入力として使用する直線については、$a^2 + b^2 = 1$ を満たすものとする(本文中ではこれを「正規形」と呼んでいる。正規形以外を想定する場合は明記する)。 これによって以下の性質が満たされる。

* $a/b = -\tanθ$ (θは直線とx軸のなす角度)
* $a = \sinθ, b = -\cosθ$ または $a = -\sinθ, b = \cosθ$

### 基本的な演算

公理に含まれないが、折紙をシミュレートするにあたって必要となる基本的な演算を列挙する。

#### (1)直線 $a_1x + b_1y + c_1 = 0$ と直線 $a_2x + b_2y + c_2 = 0$ の交点

$$
\left\{
\begin{array}{l}
x = \dfrac{b_1c_2 - b_2c_1}{a_1b_2 - a_2b_1} \\
y = - \dfrac{a_1c_2 - a_2c_1}{a_1b_2 - a_2b_1}
\end{array}
\right.
$$

#### (2)点P $(x_1, y_1)$ と点Q $(x_2, y_2)$ の距離

$$
\sqrt{(x_1 - x_2)^2 + (y_1 - y_2)^2}
$$

#### (3)点P $(x_1, y_1)$ から直線a $a_1x + b_1y + c_1 = 0$ に引いた垂線の足

$$
\left\{
\begin{array}{l}
x = x_1 -
\dfrac{a_1(a_1x_1 + b_1y_1 + c_1)}{a_1^2 + b_1^2} \\
y = y_1 - \dfrac{b_1(a_1x_1 + b_1y_1 + c_1)}{a_1^2 + b_1^2}
\end{array}
\right.
$$

特に、直線aが正規形の場合は

$$
\left\{
\begin{array}{l}
x = x_1 - a_1(a_1x_1 + b_1y_1 + c_1) \\
y = y_1 - b_1(a_1x_1 + b_1y_1 + c_1)
\end{array}
\right.
$$

#### (4)点P $(x_1, y_1)$ と直線a $a_1x + b_1y + c_1 = 0$ の距離

$$
\dfrac{| a_1x_1 + b_1y_1 + c_1 |}{\sqrt{a_1^2 + b_1^2}}
$$

特に、直線aが正規形の場合は

$$
| a_1x_1 + b_1y_1 + c_1 |
$$

#### (5)点P $(x_1, y_1)$ を直線a $a_1x + b_1y + c_1 = 0$ で折り返した点

$$
\left\{
\begin{array}{l}
x = x_1 - 2 \dfrac{a_1(a_1x_1 + b_1y_1 + c_1)}{a_1^2 + b_1^2} \\
y = y_1 - 2 \dfrac{b_1(a_1x_1 + b_1y_1 + c_1)}{a_1^2 + b_1^2}
\end{array}
\right.
$$

特に、直線aが正規形の場合は

$$
\left\{
\begin{array}{l}
x = x_1 - 2a_1(a_1x_1 + b_1y_1 + c_1) \\
y = y_1 - 2b_1(a_1x_1 + b_1y_1 + c_1)
\end{array}
\right.
$$

#### (6)直線a $a_1x + b_1y + c_1 = 0$ を直線R $a_2x + b_2y + c_2 = 0$ で折り返した直線

解となる直線の任意の点(x, y)は、演算5に従って折り返すと直線aに含まれる。よって

$$
a_2(x - 2 \dfrac{a_1(a_1x + b_1y + c_1)}{a_1^2 + b_1^2}) + b_2(y - 2 \dfrac{b_1(a_1x + b_1y + c_1)}{a_1^2 + b_1^2}) + c_2 = 0
$$

上記を整理すると、解の直線について次を得る。

$$
\left\{
\begin{array}{l}
a = a_1D - 2a_2E \\
b = b_1D - 2b_2E \\
c = c_1D - 2c_2E
\end{array}
\right.
$$

ただし、 $D = a_2^2 + b_2^2, E = a_1a_2 + b_1b_2$

##  点Pと点Qを通るように折る

点P $(x_1, y_1)$ と点Q $(x_2, y_2)$ を通る直線は、次の連立方程式を解けばよい。

$$
\left\{
\begin{array}{l}
ax_1 + by_1 + c = 0 \\
ax_2 + by_2 + c = 0 \\
\end{array}
\right.
$$

$a = 1$ として解いた後、全体に $(y_1 - y_2)$ をかけると以下を得る。

$$
\left\{
\begin{array}{l}
a = y_1 - y_2 \\
b = x_1 - x_2 \\
c = x_1y_2 - x_2y_1
\end{array}
\right.
$$

## 2. 点Pと点Qを重ねるように折る (垂直二等分線)

点P $(x_1, y_1)$ と点Q $(x_2, y_2)$ を重ねるようにして折った折線は、2点への距離が等しい点の集合になるため、直線上の任意の点 $(x, y)$ は次の性質を満たす。

$$
(x - x_1)^2 + (y - y_1)^2 = (x - x_2)^2 + (y - y_2)^2
$$

この式を整理すると、次を得る。

$$
2(x_1 - x_2)x + 2(y_1 - y_2)y + x_2^2 + y_2^2 - x_1^2 - y_1^2 = 0
$$

全体を2で割って、

$$
\left\{
\begin{array}{l}
a = x_1 - x_2 \\
b = y_1 - y_2 \\
c = \dfrac{x_2^2 + y_2^2 - x_1^2 - y_1^2}{2}
\end{array}
\right.
$$

## 3. 直線aと直線bを重ねるように折る (角の二等分線、1～2通り)

2線 $a_1x + b_1y + c_1 = 0, a_2x + b_2y + c_2 = 0$ の交点は次のように表せる。

$$
\left\{
\begin{array}{l}
x = \dfrac{b_1c_2 - b_2c_1}{a_1b_2 - a_2b_1} \\
y = \dfrac{a_1c_2 - a_2c_1}{a_2b_1 - a_1b_2}
\end{array}
\right.
$$

角の平均におけるタンジェントの公式

$$
\tan\dfrac{α + β}{2} = \dfrac{\sinα + \sinβ}{\cosα + \cosβ}
$$

より、解となる直線の傾きtanθは $- \dfrac{a_1 + a_2}{b_1 + b_2}$ または $-\dfrac{a_1 - a_2}{b_1 - b_2}$ と表せる。
また、$\tanθ = \dfrac{\sinθ}{\cosθ} = -\dfrac{a}{b}$ より、

$$
\left\{
\begin{array}{l}
a = a_1 \pm a_2 \\
b = b_1 \pm b_2
\end{array}
\right.
$$

ただし、符号は同順。
これらと交点の位置を直線の一般式に代入すると以下を得る。

$$
\left\{
\begin{array}{l}
a = a_1 \pm a_2 \\
b = b_1 \pm b_2 \\
c = c_1 \pm c_2
\end{array}
\right.
$$

2線が平行な場合、 $a_1 - a_2 = b_1 - b_2 = 0$ となるため、マイナスの符号を使った直線は存在しない。

### 補. 入力が正規形ではない場合

角の平均におけるタンジェントの公式より、解となる直線の傾きtanθは次のように表せる。

$$
-\dfrac{a_1d_2 \pm a_2d_1}{b_1d_2 \pm b_2d_1}
$$

ただし、符号は同順で $d_1 = \sqrt{a_1^2 + b_1^2}, d_2 = \sqrt{a_2^2 + b_2^2}$ 

これらと交点の位置を直線の一般式に代入すると以下を得る。

$$
\left\{
\begin{array}{l}
a = a_1d_2 \pm a_2d_1 \\
b = b_1d_2 \pm b_2d_1 \\
c = c_1d_2 \pm c_2d_1
\end{array}
\right.
$$

## 4. 直線aに垂直で、点Pを通るように折る (垂線)

この計算は入力が正規形でなくても成り立つ。

直線a $a_1x + b_1y + c_1 = 0$ に垂直で、点P $(x_2, y_2)$ を通る直線は、直線aに垂直であることから $\tan(\dfrac{π}{2} + θ) = - \dfrac{1}{\tanθ}$ より次を得る。

$$
\left\{
\begin{array}{l}
a = -b_1 \\
b = a_1
\end{array}
\right.
$$

直線の一般式に点Pを代入して解くと次を得る。

$$
\left\{
\begin{array}{l}
a = -b_1 \\
b = a_1 \\
c = b_1x_2 - a_1y_2
\end{array}
\right.
$$

### 補足: 直線aに平行で、点Pを通るように折る

この計算は入力が正規形でなくても成り立つ。

直線a $a_1x + b_1y + c_1 = 0$ に平行で、点P $(x_2, y_2)$ を通る直線は、直線aと平行であることからa, bは直線aと同じ値をとる。

直線の一般式に点Pを代入して解くと次を得る。

$$
\left\{
\begin{array}{l}
a = a_1 \\
b = b_1 \\
c = -a_1x_2 - b_1y_2
\end{array}
\right.
$$

## 5. 点Pを通り、点Qを直線aに重ねるように折る (0～2通り、2次方程式に対応)

* 直線a: $a_1x + b_1y + c_1 = 0$
* 点P: $(x_2, y_2)$
* 点Q: $(x_3, y_3)$

点Qを重ねる直線a上の点を点R(X, Y)とする。公理2より、解の直線は次のように表せる。

$$
(x_3 - X)x + (y_3 - Y)y + \dfrac{X^2 + Y^2 - x_3^2 - y_3^2}{2} = 0
$$

この直線は点Pを通るので、次の式が成り立つ。

$$
\begin{align*}
x_2(x_3 - X) + y_2(y_3 - Y) + \dfrac{X^2 + Y^2 - x_3^2 - y_3^2}{2} &= 0 \\
2x_2x_3 - 2x_2X + 2y_2y_3 - 2x_2Y + X^2 + Y^2 - x_3^2 - y_3^2 &= 0 \\
-2x_2X - 2x_2Y + X^2 + Y^2 &= -2x_2x_3 - 2y_2y_3 + x_3^2 + y_3^2
\end{align*}
$$

両辺に $x_2^2 + y_2^2$ を足す。

$$
\begin{align*}
x_2^2 - 2x_2X + X^2 + y_2^2 - 2x_2Y + Y^2 &= x_2^2 - 2x_2x_3 + x_3^2 + y_2^2 - 2y_2y_3 + y_3^2 \\
(X - x_2)^2 + (Y - y_2)^2 &= (x_2 - x_3)^2 + (y_2 - y_3)^2
\end{align*}
$$

$X, Y$ を変数、他を定数と考えた場合、この式は円を表す。

ここで、 $U = X - x_2, V = Y - y_2, W = (x_2 - x_3)^2 + (y_2 - y_3)^2$ と置く。

$$
U^2 + V^2 = W
$$

点R $(X, Y)$ は直線a上の点であるから、 $a_1X + b_1Y + c_1 = 0$ が成り立つ。よって、 $V$ を次のように表すことができる。

$$
\begin{align*}
a_1(U + x_2) + b_1(V + y_2) + c_1 &= 0 \\
b_1V &= -a_1U - a_1x_2 - b_1y_2 - c_1
\end{align*}
$$

ここで、 $N = a_1x_2 + b_1y_2 + c_1$ と置く。

$$
\begin{align*}
b_1V &= -a_1U - N \\
V &= - \dfrac{a_1U + N}{b_1}
\end{align*}
$$

これを元の式に代入する。

$$
\begin{align*}
U^2 + \dfrac{(a_1U + N)^2}{b_1^2} &= W \\
b_1^2U^2 + (a_1U + N)^2 &= b_1^2W \\
b_1^2U^2 + a_1^2U^2 + 2a_1NU + N^2 &= b_1^2W \\
(a_1^2 + b_1^2)U^2 + 2a_1NU &= b_1^2W - N^2
\end{align*}
$$

入力の直線は正規形であるから、 $a_1^2 + b_1^2 = 1$ が成り立つ。

$$
\begin{align*}
U^2 + 2a_1NU &= b_1^2W - N^2 \\
(U + a_1N)^2 &= b_1^2W - N^2 + a_1^2N^2 \\
(U + a_1N)^2 &= b_1^2W - N^2(1 - a_1^2) \\
(U + a_1N)^2 &= b_1^2W - b_1^2N^2 \\
U &= -a_1N \pm b_1\sqrt{W - N^2} \\
X &= x_2 - a_1N \pm b_1\sqrt{W - N^2}
\end{align*}
$$

$Y$ においては $a$ と $b$ , $x$ と $y$ を入れ替えれば成り立つため、

$$
\left\{
\begin{array}{l}
X = x_2 - a_1N \pm b_1\sqrt{W - N^2} \\
Y = y_2 - b_1N \mp a_1\sqrt{W - N^2}
\end{array}
\right.
$$

公理2に従ってこの点R(X, Y)と点Q(x_3, y_3)を重ねるように折ることで解を得る。

$$
\left\{
\begin{array}{l}
a = x_3 - x_2 + a_1N \mp b_1\sqrt{W - N^2} \\
b = y_3 - y_2 + b_1N \pm a_1\sqrt{W - N^2} \\
c = x_2(x_2 - x_3) + y_2(y_2 - y_3) - (a_1x_2 + b_1y_2)N \pm \dfrac{(b_1x_2 - a_1y_2)\sqrt{W - N^2}}{2}
\end{array}
\right.
$$

### 補. 入力が正規形ではない場合

$(a_1^2 + b_1^2)U^2 + 2a_1NU = b_1^2W - N^2$ から、正規形による変換ができないため以下のように計算する。

$$
\begin{align*}
(a_1^2 + b_1^2)U^2 + 2a_1NU - b_1^2W + N^2 &= 0 \\
U &= \dfrac{-a_1N \pm \sqrt{a_1^2N^2 - (a_1^2 + b_1^2)(N^2 - b_1^2W)}}{a_1^2 + b_1^2} \\
U &= \dfrac{-a_1N \pm b_1\sqrt{(a_1^2 + b_1^2)W - N^2}}{a_1^2 + b_1^2} \\
\end{align*}
$$

$$
\left\{
\begin{array}{l}
X = x_2 - \dfrac{a_1N \pm b_1\sqrt{(a_1^2 + b_1^2)W - N^2}}{a_1^2 + b_1^2} \\
Y = y_2 - \dfrac{b_1N \mp a_1\sqrt{(a_1^2 + b_1^2)W - N^2}}{a_1^2 + b_1^2}
\end{array}
\right.
$$

## 6. 点Aを直線aに重ね、点Bを直線bに重ねるように折る (1～3通り、3次方程式に対応)

* 直線a: $a_1x + b_1y + c_1 = 0$
* 直線b: $a_2x + b_2y + c_2 = 0$
* 点A: $(x_1, y_1)$
* 点B: $(x_2, y_2)$

解の直線を $Ax + By + C = 0$ とする。解の直線によって点Aを折り返すと直線aに重なるため、次が成り立つ(点B - 直線bも同様。必要になるまでB - b系は省略する)。

$$
a_1(x_1 - 2A \dfrac{Ax_1 + By_1 + C}{A^2 + B^2}) + b_1(y_1 - 2B \dfrac{Ax_1 + By_1 + C}{A^2 + B^2}) + c_1 = 0　…　(0)
$$

公理5と同様にして(0)を展開し、整理する。ここで、 $N_1 = a_1x_1 + b_1y_1 + c_1, N_2 = a_2x_2 + b_2y_2 + c_2$

$$
2(a_1A + b_1B)(x_1A + y_1B + C) - N(A^2 + B^2) = 0　…　(1)
$$

(1)をCについて整理する。


$$
\begin{align*}
 0 &= 2(a_1A + b_1B)C + 2(a_1A + b_1B)(x_1A + y_1B) - N_1(A^2 + B^2)\\
C &= - \dfrac{2(a_1A + b_1B)(x_1A + y_1B) - N_1(A^2 + B^2)}{2(a_1A + b_1B)} \\
C &= -x_1A - y_1B + \dfrac{N_1(A^2 + B^2)}{2(a_1A + b_1B)}　…　(2)
\end{align*}
$$

B - b系についても同様に整理し、代入によってCを消去する。


$$
\begin{align*}
x_1A + y_1B - \dfrac{N_1(A^2 + B^2)}{2(a_1A + b_1B)} &= x_2A + y_2B - \dfrac{N_2(A^2 + B^2)}{2(a_2A + b_2B)} \\
(a_2A + b_2B)(2(a_1A + b_1B)(x_1A + y_1B) - N_1A^2 - N_1B^2) &= (a_1A + b_1B)(2(a_2A + b_2B)(x_2A + y_2B) - N_2A^2 - N_2B^2) \\
2(a_1A + b_1B)(a_2A + b_2B)(x_1A + y_1B - x_2A - y_2B) + (a_1A + b_1B)(N_2A^2 + N_2B^2) - (a_2A + b_2B)(N_1A^2 + N_1B^2) &= 0
\end{align*}
$$

$X = x_1 - x_2, Y = y_1 - y_2$ と置く。

$$
\begin{align*}
2(a_1A + b_1B)(a_2A + b_2B)(XA + YB) + (a_1A + b_1B)(N_2A^2 + N_2B^2) - (a_2A + b_2B)(N_1A^2 + N_1B^2) &= 0 \\
2(a_1a_2A^2 + (a_1b_2 + a_2b_1)AB + b_1b_2B^2)(XA + YB) + (a_1N_2A^3 + a_1N_2AB^2 + b_1N_2A^2B + b_1N_2B^3) - (a_2N_1A^3 + a_2N_1AB^2 + b_2N_1A^2B + b_2N_1B^3) &= 0 \\
(a_1N_2 - a_2N_1 + 2a_1a_2X)A^3 + (b_1N_2 - b_2N_1 + 2(a_1b_2 + a_2b_1)X + 2a_1a_2Y)A^2B + (a_1N_2 - a_2N_1 + 2(a_1b_2 + a_2b_1)Y + 2b_1b_2X)AB^2 + (b_1N_2 - b_2N_1 + 2b_1b_2Y)B^3 &= 0
\end{align*}
$$

$D = a_1N_2 - a_2N_1, E = b_1N_2 - b_2N_1, F = a_1b_2 + a_2b_1$ と置く。

$$
(D + 2a_1a_2X)A^3 + (E + 2FX + 2a_1a_2Y)A^2B + (D + 2FY + 2b_1b_2X)AB^2 + (E + 2b_1b_2Y)B^3 = 0
$$

$B = 1$ として整理する。

$$
(D + 2a_1a_2X)A^3 + (E + 2FX + 2a_1a_2Y)A^2 + (D + 2FY + 2b_1b_2X)A + E + 2b_1b_2Y = 0
$$

この三次方程式を解くことでAを求めることができる。 $D + 2a_1a_2X = 0$ となる場合はx座標系とy座標系を入れ替えて解くことができる。

## 7. 直線aに垂直で、点Pを直線bに重ねるように折る

* 直線a: $a_1x + b_1y + c_1 = 0$
* 直線b: $a_2x + b_2y + c_2 = 0$
* 点P: $(x_3, y_3)$

解の直線は直線aに垂直であることから、 $\tan(\dfrac{π}{2} + θ) = - \dfrac{1}{\tanθ}$
より次を得る。

$$
\left\{
\begin{array}{l}
a = -b_1 \\
b = a_1
\end{array}
\right.
$$

解の直線によって点Pを折り返した点は直線bに重なることから、解の直線を $-b_1x + a_1y + c = 0$ とすると次が成り立つ。

$$
a_2(x_3 + 2b_1 \dfrac{a_1y_3 - b_1x_3 + c}{a_1^2 + b_1^2}) + b_2(y_3 - 2a_1 \dfrac{a_1y_3 - b_1x_3 + c}{a_1^2 + b_1^2}) + c_2 = 0
$$

これをcについて解くと次を得る。

$$
\begin{align*}
2a_2b_1(a_1y_3 - b_1x_3 + c) - 2a_1b_2(a_1y_3 - b_1x_3 + c) + (a_2x_3 + b_2y_3 + c_2)(a_1^2 + b_1^2) &= 0 \\
2(a_2b_1 - a_1b_2)c &= -2(a_2b_1 - a_1b_2)(a_1y_3 - b_1x_3) - (a_2x_3 + b_2y_3 + c_2)(a_1^2 + b_1^2) \\
c &= b_1x_3 - a_1y_3 - \dfrac{(a_2x_3 + b_2y_3 + c_2)(a_1^2 + b_1^2)}{2(a_2b_1 - a_1b_2)}
\end{align*}
$$

$a_1^2 + b_1^2 = 1$ なので(入力が正規形ではない場合、このステップを省略する)、

$$
\left\{
\begin{array}{l}
a = -b_1 \\
b = a_1 \\
c = b_1x_3 - a_1y_3 - \dfrac{a_2x_3 + b_2y_3 + c_2}{2(a_2b_1 - a_1b_2)}
\end{array}
\right.
$$