module Orimath.Basics.View.UniversalColor
open System.Windows.Media
open Orimath.Combination

let private createBrush (value: uint) =
    let brush = SolidColorBrush(Color.FromRgb(byte(value >>> 16), byte(value >>> 8), byte(value)))
    brush.Freeze()
    brush

let brushes =
    let brushes = Array.zeroCreate 20
    brushes.[int InstructionColor.Black] <- createBrush(0x000000u)
    brushes.[int InstructionColor.White] <- createBrush(0xFFFFFFu)
    brushes.[int InstructionColor.LightGray] <- createBrush(0xC8C8CBu)
    brushes.[int InstructionColor.Gray] <- createBrush(0x7F878Fu)
    brushes.[int InstructionColor.Red] <- createBrush(0xFF2800u)
    brushes.[int InstructionColor.Yellow] <- createBrush(0xFAF500u)
    brushes.[int InstructionColor.Green] <- createBrush(0x35A16Bu)
    brushes.[int InstructionColor.Blue] <- createBrush(0x0041FFu)
    brushes.[int InstructionColor.Skyblue] <- createBrush(0x66CCFFu)
    brushes.[int InstructionColor.Pink] <- createBrush(0xFF99A0u)
    brushes.[int InstructionColor.Orange] <- createBrush(0xFF9900u)
    brushes.[int InstructionColor.Purple] <- createBrush(0x9A0079u)
    brushes.[int InstructionColor.Brown] <- createBrush(0x663300u)
    brushes.[int InstructionColor.LightPink] <- createBrush(0xFFD1D1u)
    brushes.[int InstructionColor.Cream] <- createBrush(0xFFFF99u)
    brushes.[int InstructionColor.YellowGreen] <- createBrush(0xCBF266u)
    brushes.[int InstructionColor.LightSkyblue] <- createBrush(0xB4EBFAu)
    brushes.[int InstructionColor.Beige] <- createBrush(0xEDC58Fu)
    brushes.[int InstructionColor.LightGreen] <- createBrush(0x87E7B0u)
    brushes.[int InstructionColor.LightPurple] <- createBrush(0xC7B2DEu)
    brushes
    
let getBrush (instructionColor: InstructionColor) = brushes.[int instructionColor]

let getColor instructionColor = (getBrush instructionColor).Color
