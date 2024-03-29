﻿Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports NeuralProject.Interfaces


Namespace NeuralProject

    ''' <summary>Класс предоставляет методы для сериализации и десериализации нейросети</summary>
    Public Class NeuralState

        Private Shared regExpSection As New Regex("\[(.*?)\]([\W\w]+?(?:\r{2,}|\n{2,}|$))", RegexOptions.Compiled Or RegexOptions.Multiline)
        Private Shared regExpNetwork As New Regex("\s*Layers\s*=\s*(.*)$", RegexOptions.Compiled Or RegexOptions.IgnoreCase Or RegexOptions.Multiline)
        Private Shared regExpArrayAF As New Regex("\((\d+)\)\((\d+)\)\s*=\s*([A-z]+)", RegexOptions.Compiled Or RegexOptions.IgnoreCase Or RegexOptions.Multiline)
        Private Shared regExpArray1D As New Regex("\((\d+)\)\((\d+)\)\s*=\s*([\d|\.|\-|E+]+)", RegexOptions.Compiled Or RegexOptions.IgnoreCase Or RegexOptions.Multiline)
        Private Shared regExpArray2D As New Regex("\((\d+)\)\((\d+)\)\((\d+)\)\s*=\s*([\d|\.|\-|E+]+)", RegexOptions.Compiled Or RegexOptions.IgnoreCase Or RegexOptions.Multiline)

        ' Метод сериализует массив формата Double()()
        Private Shared Function NeuronSerializer(Writer As StringBuilder, Section As String, Data As Double()()) As StringBuilder
            Writer.AppendLine($"[{Section}]")

            For idxLayer = 0 To Data.GetUpperBound(0)
                For idxNeuron = 0 To Data(idxLayer).GetUpperBound(0)
                    Writer.AppendLine($"({idxLayer})({idxNeuron})={Data(idxLayer)(idxNeuron)}")
                Next
            Next

            Return Writer
        End Function

        ' Метод сериализует массив формата Double()()()
        Private Shared Function WeightSerializer(Writer As StringBuilder, Section As String, Data As Double()()()) As StringBuilder
            Writer.AppendLine($"[{Section}]")

            For idxLayer = 0 To Data.GetUpperBound(0)
                For M = 0 To Data(idxLayer).GetUpperBound(0)
                    For N = 0 To Data(idxLayer)(M).GetUpperBound(0)
                        Writer.AppendLine($"({idxLayer})({M})({N})={Data(idxLayer)(M)(N)}")
                    Next
                Next
            Next

            Return Writer
        End Function

        ' Метод сериализует массив формата ActivatorFunction()()
        Private Shared Function ActivatorSerializer(Writer As StringBuilder, Section As String, Data As IFunction()()) As StringBuilder
            Writer.AppendLine($"[{Section}]")

            For idxLayer = 0 To Data.GetUpperBound(0)
                For idxNeuron = 0 To Data(idxLayer).GetUpperBound(0)
                    Writer.AppendLine($"({idxLayer})({idxNeuron})={Data(idxLayer)(idxNeuron).Name}")
                Next
            Next

            Return Writer
        End Function

        ' Метод сериализует параметры сети
        Private Shared Function NetworkSerializer(Writer As StringBuilder, Section As String, Layers As String) As StringBuilder
            Writer.AppendLine($"[{Section}]")
            Writer.AppendLine($"Version=3")
            Writer.AppendLine($"Layers={Layers}")

            Return Writer
        End Function

        ' Метод десериализует параметры сети в сеть
        Private Shared Function NetworkDeserializer(SectionBody As String) As INetwork
            Dim mNetwork = regExpNetwork.Match(SectionBody)
            Dim Layers = mNetwork.Groups(1).Value.Replace(vbCr, "").Split(","c).Select(Function(x) CInt(x)).ToArray()

            ' Первичное создание сети
            Dim retValue = New NeuralNetwork(Layers)

            Return retValue
        End Function

        ' Метод десериализует массив Double()()
        Private Shared Function NeuronDeserializer(Network As INetwork, SectionBody As String) As INetwork
            For Each xMatch As Match In regExpArray1D.Matches(SectionBody)
                Network.Neurons(CInt(xMatch.Groups(1).Value))(CInt(xMatch.Groups(2).Value)) = CDbl(xMatch.Groups(3).Value)
            Next

            Return Network
        End Function

        ' Метод десериализует массив Double()()
        Private Shared Function ActivityDeserializer(Network As INetwork, SectionBody As String) As INetwork
            For Each xMatch As Match In regExpArray1D.Matches(SectionBody)
                Network.Activities(CInt(xMatch.Groups(1).Value))(CInt(xMatch.Groups(2).Value)) = CDbl(xMatch.Groups(3).Value)
            Next

            Return Network
        End Function

        ' Метод десериализует массив Double()()
        Private Shared Function ErrorDeserializer(Network As INetwork, SectionBody As String) As INetwork
            For Each xMatch As Match In regExpArray1D.Matches(SectionBody)
                Network.Errors(CInt(xMatch.Groups(1).Value))(CInt(xMatch.Groups(2).Value)) = CDbl(xMatch.Groups(3).Value)
            Next

            Return Network
        End Function

        ' Метод десериализует массив Double()(,)
        Private Shared Function WeightDeserializer(Network As INetwork, SectionBody As String) As INetwork
            For Each xMatch As Match In regExpArray2D.Matches(SectionBody)
                Network.Weights(CInt(xMatch.Groups(1).Value))(CInt(xMatch.Groups(2).Value))(CInt(xMatch.Groups(3).Value)) = CDbl(xMatch.Groups(4).Value)
            Next

            Return Network
        End Function

        ' Метод десериализует массив ActivatorFunction()()
        Private Shared Function ActivatorDeserializer(Network As INetwork, SectionBody As String) As INetwork
            Dim functionList = NeuralNetwork.RegenerateFunctionList()

            For Each xMatch As Match In regExpArrayAF.Matches(SectionBody)
                Network.Activators(CInt(xMatch.Groups(1).Value))(CInt(xMatch.Groups(2).Value)) = functionList(xMatch.Groups(3).Value)
            Next

            Return Network
        End Function

        ''' <summary>Сохранить параметры нейросети в файл</summary>
        Public Shared Sub Save(FileName As String, Network As INetwork)

            'Меняем формат для потока, для корректной записи чисел
            Dim myCulture = Globalization.CultureInfo.CurrentCulture
            Threading.Thread.CurrentThread.CurrentCulture = Globalization.CultureInfo.InvariantCulture

            ' Собираем параметры сети в строку инициализации
            Dim getLayers As Func(Of INetwork, String) =
                Function(source As INetwork) As String
                    Dim ret As String = String.Empty
                    For n = 0 To source.Neurons.GetUpperBound(0)
                        If n <> source.Neurons.GetUpperBound(0) Then
                            ret &= source.Neurons(n).Length & ", "
                        Else
                            ret &= source.Neurons(n).Length
                        End If
                    Next
                    Return ret
                End Function

            ' Собираем секции
            Dim sbWriter As New StringBuilder

            sbWriter = NetworkSerializer(sbWriter, "Network", getLayers(Network))
            sbWriter.AppendLine()

            sbWriter = NeuronSerializer(sbWriter, "Neurons", Network.Neurons)
            sbWriter.AppendLine()

            sbWriter = ActivatorSerializer(sbWriter, "Activators", Network.Activators)
            sbWriter.AppendLine()

            sbWriter = NeuronSerializer(sbWriter, "Activities", Network.Activities)
            sbWriter.AppendLine()

            sbWriter = NeuronSerializer(sbWriter, "Errors", Network.Errors)
            sbWriter.AppendLine()

            sbWriter = WeightSerializer(sbWriter, "Weights", Network.Weights)
            sbWriter.AppendLine()

            File.WriteAllText(FileName, sbWriter.ToString(), Encoding.UTF8)

            'Возвращаем формат для потока
            Threading.Thread.CurrentThread.CurrentCulture = myCulture
        End Sub

        ''' <summary>Загрузить параметры нейросети из файла</summary>
        Public Shared Function Load(FileName As String) As INetwork

            'Меняем формат для потока, для корректного чтения чисел
            Dim myCulture = Globalization.CultureInfo.CurrentCulture
            Threading.Thread.CurrentThread.CurrentCulture = Globalization.CultureInfo.InvariantCulture

            Dim fileBody = File.ReadAllText(FileName, Encoding.UTF8)
            Dim resultNetwork As INetwork = Nothing

            ' Смотрим все найденные секции
            For Each mSection As Match In regExpSection.Matches(fileBody.Replace(vbLf, String.Empty))

                ' Тело секции. { Костыль для регулярки секций: замена LF на пустоту, замена CR на CRLF}
                Dim sectionBody As String = mSection.Groups(2).Value.Replace(vbCr, vbCrLf)

                ' По имени секции
                Select Case mSection.Groups(1).Value

                    Case "Network"
                        resultNetwork = NetworkDeserializer(sectionBody)

                    Case "Neurons"
                        resultNetwork = NeuronDeserializer(resultNetwork, sectionBody)

                    Case "Activators"
                        resultNetwork = ActivatorDeserializer(resultNetwork, sectionBody)

                    Case "Activities"
                        resultNetwork = ActivityDeserializer(resultNetwork, sectionBody)

                    Case "Errors"
                        resultNetwork = ErrorDeserializer(resultNetwork, sectionBody)

                    Case "Weights"
                        resultNetwork = WeightDeserializer(resultNetwork, sectionBody)

                End Select
            Next

            'Возвращаем формат для потока
            Threading.Thread.CurrentThread.CurrentCulture = myCulture

            Return resultNetwork
        End Function

    End Class


End Namespace