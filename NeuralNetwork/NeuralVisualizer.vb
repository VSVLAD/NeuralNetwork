Option Explicit On
Option Strict On

Imports System.Drawing
Imports System.Windows.Forms

Namespace NeuralProject

    Public Class NeuralVisualizer

        Private bmpBuffer As Bitmap
        Private g As Graphics

        Private renderWidth As Integer
        Private renderHeight As Integer
        Private layerShiftX() As Integer

        Private neuronRadius As Integer = 40
        Private neuronBorderWidth As Integer = 2
        Private neuronBorderColor As Color = Color.Black
        Private neuronBackgroundColor As Color = Color.WhiteSmoke
        Private neuronFontColor As Color = Color.Black

        Dim penNeuronBorder As New Pen(neuronBorderColor, neuronBorderWidth)
        Dim brushNeuronFont As New SolidBrush(neuronFontColor)
        Dim brushNeuronBack As New SolidBrush(neuronBackgroundColor)

        Public Sub New(RenderBox As PictureBox, Network As NeuralNetwork)
            renderWidth = RenderBox.Width
            renderHeight = RenderBox.Height

            ReDim layerShiftX(Network.Neurons.Length - 1)
            For indLayer = 0 To layerShiftX.GetUpperBound(0)

            Next

            bmpBuffer = New Bitmap(renderWidth, renderHeight)
            g = Graphics.FromImage(bmpBuffer)
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

            RenderBox.Image = bmpBuffer


        End Sub

        Public Sub ResizeRenderBox(RenderBox As PictureBox)
            If RenderBox Is Nothing Then Return


        End Sub

        Public Sub DrawLayer(LayerIndex As Integer)
            For neuronIndex = 1 To 5
                DrawNeuron(LayerIndex, neuronIndex, -1 + Rnd() * 2)
            Next
        End Sub

        Public Sub DrawNeuron(LayerIndex As Integer, NeuronIndex As Integer, Value As Double)
            Dim leftX As Single = CSng(LayerIndex * (renderWidth / (3 + 1)) - neuronRadius / 2)
            Dim topY As Single = CSng(NeuronIndex * (renderHeight / (5 + 1)) - neuronRadius / 2)

            g.FillEllipse(brushNeuronBack, leftX, topY, neuronRadius, neuronRadius)
            g.DrawEllipse(penNeuronBorder, leftX, topY, neuronRadius, neuronRadius)

            g.DrawString(Math.Round(Value, 2).ToString("0.##"), SystemFonts.DefaultFont, brushNeuronFont, CSng(leftX + neuronRadius / 2 - 12), CSng(topY + neuronRadius / 2 - 6))
        End Sub

    End Class


End Namespace