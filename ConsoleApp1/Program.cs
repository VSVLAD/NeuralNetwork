﻿using System;
using System.Text.RegularExpressions;

public class Example
{
    public static void Main()
    {
        string pattern = @"\[(.*)\]$[\w\W]+?(\n\n|\z)";
        string input = @"[NeuralNetwork]
Layers=2, 6, 6, 1
LearningRate=0.14
Epoch=159689

[Neurons]
(0)(0)=0.014
(0)(1)=0.014
(1)(0)=0.544438532806638
(1)(1)=0.489151470336042
(1)(2)=0.546775162113461
(1)(3)=0.455486208527701
(1)(4)=0.455898103278486
(1)(5)=0.405111576750808
(2)(0)=0.00475100721226533
(2)(1)=0.0113149830432007
(2)(2)=0.0051096936857983
(2)(3)=0.00500206839303268
(2)(4)=0.593319072953235
(2)(5)=0.827192976555574
(3)(0)=0.0894062965546832

[Activators]
(0)(0)=SIGMOID
(0)(1)=SIGMOID
(1)(0)=SIGMOID
(1)(1)=SIGMOID
(1)(2)=SIGMOID
(1)(3)=SIGMOID
(1)(4)=SIGMOID
(1)(5)=SIGMOID
(2)(0)=SIGMOID
(2)(1)=SIGMOID
(2)(2)=SIGMOID
(2)(3)=SIGMOID
(2)(4)=SIGMOID
(2)(5)=SIGMOID
(3)(0)=SIGMOID

[Errors]
(0)(0)=0
(0)(1)=0
(1)(0)=0.294131847274254
(1)(1)=-0.0838797049496053
(1)(2)=0.292013174142074
(1)(3)=-0.346541719894334
(1)(4)=-0.334074598586551
(1)(5)=-0.74912883538046
(2)(0)=0.00983146336323089
(2)(1)=0.032362530747083
(2)(2)=0.0130355995960677
(2)(3)=0.00925351437352529
(2)(4)=-0.0252238882502201
(2)(5)=-0.00665521458957127
(3)(0)=0.00859370344531682

[Weights]
(0)(0,0)=6.35994939572515
(0)(0,1)=-1.34410926949169
(0)(0,2)=6.87298312543509
(0)(0,3)=-6.61752226663077
(0)(0,4)=-6.71413912564308
(0)(0,5)=-12.9380281087751
(0)(1,0)=6.37065061623738
(0)(1,1)=-1.75603932388089
(0)(1,2)=6.53082529383405
(0)(1,3)=-6.13480266379254
(0)(1,4)=-5.91955805209331
(0)(1,5)=-14.5063465038673
(1)(0,0)=-1.75886047650144
(1)(0,1)=2.34640403405095
(1)(0,2)=-0.331828427850592
(1)(0,3)=-1.69600726373769
(1)(0,4)=-8.85971371671871
(1)(0,5)=-4.81499705297864
(1)(1,0)=-1.2303044647967
(1)(1,1)=-1.46271074378443
(1)(1,2)=-1.20424082305532
(1)(1,3)=-1.88054228493414
(1)(1,4)=-0.525776898790743
(1)(1,5)=0.690831655264092
(1)(2,0)=-2.39934261552756
(1)(2,1)=2.46000999803277
(1)(2,2)=-1.41255799583572
(1)(2,3)=-1.64403037935171
(1)(2,4)=-9.20156412041232
(1)(2,5)=-5.63929450643669
(1)(3,0)=-1.95678309621885
(1)(3,1)=-3.57569461434879
(1)(3,2)=-3.02308470834063
(1)(3,3)=-2.30011219753623
(1)(3,4)=5.11677617012186
(1)(3,5)=3.2782213109526
(1)(4,0)=-1.45283150783146
(1)(4,1)=-3.45513345670423
(1)(4,2)=-2.35005787917164
(1)(4,3)=-2.32658746677093
(1)(4,4)=5.3037179652364
(1)(4,5)=3.30857863050462
(1)(5,0)=-2.27023544704653
(1)(5,1)=-7.8333135248654
(1)(5,2)=-3.16221920569969
(1)(5,3)=-1.09196368670233
(1)(5,4)=14.1685986840703
(1)(5,5)=9.70372081413007
(2)(0,0)=1.14403149060654
(2)(1,0)=3.76584326854688
(2)(2,0)=1.51687848901049
(2)(3,0)=1.07677890479726
(2)(4,0)=-2.93510114541984
(2)(5,0)=-0.774348142778798
";
        RegexOptions options = RegexOptions.Multiline;

        foreach (Match m in Regex.Matches(input, pattern, options))
        {
            Console.WriteLine("'{0}' found at index {1}.", m.Value, m.Index);
        }
    }
}