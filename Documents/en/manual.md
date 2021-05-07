# Orimath

TARGET VERSION: 1.0.0

THIS IS TRANSLATED VERSION. ORIGINAL IS [JAPANESE](../ja/manual.md).

* [Summary](#Summary)
* [System requirements](#System-requirements)
* [UI description](#UI-description)

Tools and commands

* [Folding tool](#-Folding-tool)
  * [Select a point or line (Left click)](#Select-a-point-or-line-Left-click)
  * [Make a crease (drag)](#Make-a-crease-drag)
  * [Fold (Shift key)](#Fold-Shift-key)
  * [Make a crease in only the front-most paper (Ctrl key)](#Make-a-crease-in-only-the-front-most-paper-Ctrl-key)
  * [Fold only the front-most paper (Ctrl key + Shift key)](#Fold-only-the-front-most-paper-Ctrl-key--Shift-key)
  * [Ignore alignments and fold freely (Alt key)](#Ignore-alignments-and-fold-freely-Alt-key)
  * [Fold so that two elements overlap (Left drag)](#Fold-so-that-two-elements-overlap-Left-drag)
  * [Fold through the two elements (right drag)](#Fold-through-the-two-elements-right-drag)
* [Measurement tool](#-Measurement-tool)
* [Commands](#Commands)
* [Other menu items](#Other-menu-items)

## Summary

| Name | Type | Author |
| --- | --- | --- |
| Orimath | Origami simulator | 豊穣ミノリ (Hojo Minori) |

Orimath is a math-focused *origami simulator.* You can fold paper with intuitive operations just like real origami, and you can check the numerically information of points and lines on the paper.

Orimath is not designed to be used primarily for making crease patterns and folding diagrams, but I plan to add useful features in the future.

## System requirements

| OS | Framework | Memory | Storage | CPU |
| --- | --- | --- | --- | --- |
| Windows 10 | .NET5 | > 128MB | > 6MB | unspecified |

# UI description

![UI description](../Images/view2.png?raw=true)

### ① Origami view

Main workspace. Click or drag here to edit the origami.

### ② Crease pattern (CP)

Displays crease pattern of the current paper.

### ③ Measurement view

Displays numerically information in the coordinate system with the lower left of the paper as the origin and the center of the paper as (0.5, 0.5) about selected point or line.

| variable | means |
| --- | --- |
| *X, Y* | position of the selected point. |
| *θ* | leftward angle with 0° at the horizontal of the selected line |
| *d* | distance of the selected line from the origin |
| *l* | length of the selected line |

### ④ Tools

Click on the buttons to switch tools.

### ⑤ Menu and commands

Click on the buttons to execute commands (e.g. undo, turn over).

### ⑥ Operation navigation

Displays operation navigations of folding tool.

# Tools and commands

## ![icon](../../Plugins/Orimath.Basics/Icons/fold.png?raw=true) Folding tool

![view](../Images/view.png?raw=true)

Use the left and right mouse buttons in combination with the `Ctrl` , `Shift` , and `Alt` keys. You can make a crease or fold the paper.

| Operation | Action |
| --- | --- |
| Click | Select a point or line |
| Left drag | Fold so that two elements overlap |
| Right drag | Fold through the two elements |
| `Ctrl` key | Fold only the front-most paper |
| `Shift` key | Fold (release Shift key to make a crease) |
| `Alt` key | Ignore alignments and fold freely |

### Select a point or line (Left click)

Click on a point or line of the paper to select that.
Numerically information about selection is displayed in **Measurement view**, it also affects the [fold by left dragging](#Fold-so-that-two-elements-overlap-Left-drag).

### Make a crease (drag)

![view](../Images/fold1.gif?raw=true)

When you drag left from the *element* (a line or point) to the other element, a blue dotted line and an arrow will appear.
If you release the mouse button while the **blue** dotted line is displayed, you **can** make a crease where the dotted line was. If you release the button when only the **gray** dotted line is displayed, you **cannot** make a crease.

In this way, Orimath shows the predicted fold as a fold diagram while dragging, and confirms the fold when the mouse button is released.

### Fold (Shift key)

![view](../Images/fold3.png?raw=true)

When you hold down `Shift` key and drag, a one-way blue arrow will appear. Hold down the `Shift` key and release the mouse button, to fold paper in the direction of the arrow.
The folding direction is the same as the dragging direction.

### Make a crease in only the front-most paper (Ctrl key)

![view](../Images/fold2.png?raw=true)

Hold down `Ctrl` key and drag, to make a crease in only the front-most paper.

### Fold only the front-most paper (Ctrl key + Shift key)

![view](../Images/fold4.png?raw=true)

By combining the `Shift` key and `Ctrl` key, you can fold (instead of making a crease) only the front-most paper.

### Ignore alignments and fold freely (Alt key)

![view](../Images/fold5.png?raw=true)

In Orimath, only aligned fold can be usually confirmed. Press `Alt` key to ignore alignments and fold freely.
You can use it in combination with the `Shift` and/or `Ctrl` keys.

### Fold so that two elements overlap (Left drag)

Drag left from element to element, to fold so that two elements overlap.

#### Fold the point to the point

![view](../Images/axiom2.png?raw=true)

Drag left from point to point, to fold so that two points overlap.

#### Fold the line to the line

![view](../Images/axiom3.png?raw=true)

Drag left from line to line, to fold so that two lines overlap.

If there is more than one way to fold, other candidates will be shown with gray dotted lines.
The fold with the closest overlap to the dragged position will be selected from the candidates.

#### Fold the point to the line

![view](../Images/axiom8.png?raw=true)

Drag left from point to line or line to point, to fold so that the point and line overlap.
There are variations on this folding method, and the behavior changes when a point or line is selected before dragging.

---

![view](../Images/axiom5.png?raw=true)

When a point is selected, to fold **through that point** .

---

![view](../Images/axiom7.png?raw=true)

When a line is selected, to fold **perpendicular to that line** .

---

![view](../Images/axiom6.png?raw=true)

Hold down `Shift` key and click on the elements, you can select one point and one line at a time.
In this state, drag from a point to a line, to fold so that [the selected point and line] and [the dragging point and line] overlap each other.

### Fold through the two elements (right drag)

Drag right from element to element, to fold through the two elements.

#### Fold through the two points

![view](../Images/axiom1.png?raw=true)

Drag right from point to point, to fold through the two points.

---

#### Fold through the point and perpendicular to the line

![view](../Images/axiom4.png?raw=true)

Drag right from point to line or line to point, to fold through the point and perpendicular to the line.

## ![icon](../../Plugins/Orimath.Basics/Icons/measure.png?raw=true) Measurement tool

![view](../Images/measure.png?raw=true)

By dragging points and lines on the paper, you can find their distances and intersections.

## Commands

Click on the buttons lined up at the top of the window to perform operations.


### ![icon](../../Plugins/Orimath.Basics/Icons/open_file.png?raw=true) Open (Ctrl+O)

Open a .orimath format file.

### ![icon](../../Plugins/Orimath.Basics/Icons/save_file_as.png?raw=true) Save as (Ctrl+Shift+S)

Save the current paper to a new .orimath format file.

### ![icon](../../Plugins/Orimath.Basics/Icons/save_file.png?raw=true) Save (Ctrl+S)

Save the current paper to the .orimath format file.

### ![icon](../../Plugins/Orimath.Basics/Icons/undo.png?raw=true) Undo (Ctrl+Z)

Undo last operation.

### ![icon](../../Plugins/Orimath.Basics/Icons/redo.png?raw=true) Redo (Ctrl+Y)

Redo undone operation.

### ![icon](../../Plugins/Orimath.Basics/Icons/rotate_l.png?raw=true) Rotate 90° left (Ctrl+←)

Rotate the paper 90° to the left.

### ![icon](../../Plugins/Orimath.Basics/Icons/rotate_r.png?raw=true) Rotate 90° right (Ctrl+→)

Rotate the paper 90° to the right.

### ![icon](../../Plugins/Orimath.Basics/Icons/turn_v.png?raw=true) Turn over vertically (Ctrl+↑)

Turn the paper over vertically.

### ![icon](../../Plugins/Orimath.Basics/Icons/turn_h.png?raw=true) Turn over horizontally (Ctrl+↓)

Turn the paper over horizontally.

### ![icon](../../Plugins/Orimath.Basics/Icons/open_all.png?raw=true) Unfold all (Ctrl+E)

Unfold completely the folded paper. Also, paper return to its initial orientation.

### ![icon](../../Plugins/Orimath.Basics/Icons/new_paper.png?raw=true) New paper (Ctrl+N)

Discard the current paper and create a new one.
Using this command, you can change the paper to **square, rectangle, or regular polygon** .

### ![icon](../../Plugins/Orimath.Basics/Icons/delete.png?raw=true) Reset (Ctrl+Delete)

Remove all creases and folds, and return to a clean paper.

### ![icon](../../Plugins/Orimath.Basics.View/Icons/export_png.png?raw=true) Export in png image

Export the current paper in a png image.

### ![icon](../../Plugins/Orimath.Basics.View/Icons/export_svg.png?raw=true) Export in svg image

Export the current paper in a svg image.

### ![icon](../../Plugins/Orimath.Basics.View/Icons/inst_list.png?raw=true) Show instructions (Ctrl+D)

Show the origami instructions of the current paper. Diagrams is the same as that is displayed on during the fold operation.

## Other menu items

### Setting-Preference

Change the display size of the paper.

### Setting-Plugin settings

Switch the enability of features.

### Help-Help

Display this page.

### Help-Version info

Display version infomation of running Orimath.
