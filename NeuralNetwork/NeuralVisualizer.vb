Option Explicit On
Option Strict On

Imports System.Drawing
Imports System.Windows.Forms

Imports NeuralProject.Interfaces

Namespace NeuralProject

    Public Class NeuralVisualizer

        Private WithEvents userNetwork As INetwork
        Private WithEvents boxRender As PictureBox

        Private bmpBuffer As Bitmap
        Private g As Graphics

        Private renderWidth As Integer
        Private renderHeight As Integer
        Private layerShiftX() As Integer

        ' Параметры кистей и перьев
        Private neuronRadius As Integer = 40
        Private neuronBorderWidth As Integer = 2
        Private neuronBorderColor As Color = Color.Black
        Private neuronBackgroundColor As Color = Color.WhiteSmoke
        Private neuronFontColor As Color = Color.Black

        ' Кистья и перья для рисования
        Dim penNeuronBorder As New Pen(neuronBorderColor, neuronBorderWidth)
        Dim brushNeuronFont As New SolidBrush(neuronFontColor)
        Dim brushNeuronBack As New SolidBrush(neuronBackgroundColor)

        ' Был ли инициализирован класс
        Public Function IsInitialized() As Boolean
            Return userNetwork IsNot Nothing
        End Function

        ' Инциализация буфера
        Public Sub InitRender(RenderBox As PictureBox, Network As INetwork)
            Me.boxRender = RenderBox
            Me.userNetwork = Network

            Me.renderWidth = boxRender.Width
            Me.renderHeight = boxRender.Height

            ' Если старый буфер был живой, уничтожаем
            If bmpBuffer IsNot Nothing Then bmpBuffer.Dispose()

            ' Создаём буфер на котором будем рисовать
            bmpBuffer = New Bitmap(renderWidth, renderHeight)
            g = Graphics.FromImage(bmpBuffer)
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

            ' Картинкой будет сам буфер, контрол будет сам перерисовывать
            boxRender.Image = bmpBuffer

            ' Базовая отрисовка
            DrawAll()
        End Sub

        ''' <summary>Отрисовать всё</summary>
        Public Sub DrawAll()
            For LayerIndex = 0 To userNetwork.Neurons.GetUpperBound(0)
                DrawLayer(LayerIndex)
            Next
        End Sub

        ''' <summary>Отрисовать слой с нейронами</summary>
        ''' <param name="LayerIndex">Номер слоя</param>
        Public Sub DrawLayer(LayerIndex As Integer)
            For NeuronIndex = 0 To userNetwork.Neurons(LayerIndex).GetUpperBound(0)
                DrawNeuron(LayerIndex, NeuronIndex, userNetwork.Neurons(LayerIndex)(NeuronIndex))
            Next
        End Sub

        ''' <summary>Отрисовать нейрон</summary>
        ''' <param name="LayerIndex">Номер слоя</param>
        ''' <param name="NeuronIndex">Номер нейрона</param>
        ''' <param name="Value">Значение сигнала</param>
        Public Sub DrawNeuron(LayerIndex As Integer, NeuronIndex As Integer, Value As Double)

            ' + 1 чтобы при перемножении ровно сдвинули
            Dim leftX As Single = CSng((LayerIndex + 1) * (renderWidth / (userNetwork.Neurons.Length + 1)) - neuronRadius / 2)
            Dim topY As Single = CSng((NeuronIndex + 1) * (renderHeight / (userNetwork.Neurons(LayerIndex).Length + 1)) - neuronRadius / 2)

            g.FillEllipse(brushNeuronBack, leftX, topY, neuronRadius, neuronRadius)
            g.DrawEllipse(penNeuronBorder, leftX, topY, neuronRadius, neuronRadius)

            g.DrawString(Math.Round(Value, 2).ToString("0.##"), SystemFonts.DefaultFont, brushNeuronFont, CSng(leftX + neuronRadius / 2 - 12), CSng(topY + neuronRadius / 2 - 6))
        End Sub

        ''' <summary>Отрисовать веса синапсов</summary>
        ''' <param name="LayerFrom">Слой откуда</param>
        ''' <param name="LayerTo">Слой куда</param>
        Public Sub DrawSynapse(LayerFrom As Integer, LayerTo As Integer)

        End Sub


        ' Перерисовка сигналов
        Private Sub userNetwork_ForwardComplete(LayerIndex As Integer) Handles userNetwork.ForwardComplete
            DrawLayer(LayerIndex)

            boxRender.Invalidate()
            Application.DoEvents()
        End Sub

        ' Перерисовка ошибок
        Private Sub userNetwork_BackwardComplete(LayerIndex As Integer) Handles userNetwork.BackwardComplete
            '
        End Sub

    End Class

End Namespace