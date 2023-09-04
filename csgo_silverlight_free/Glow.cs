using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.VisualBasic.CompilerServices;
using System.Runtime.CompilerServices;

namespace csgo_silverlight_free
{
    public class Glow
    {
        public struct GlowStruct
        {
            public float r;
            public float g;
            public float b;
            public float a;
            public bool rwo;
            public bool rwuo;
        }

        private static void DrawGlow(UInt32 pGlowIn, GlowStruct col)
        {
            object objectValue = RuntimeHelpers.GetObjectValue(Mem.ReadInt(MainPage.clientBase + Offsets.OFSSET_GlowObject, 4));
            Mem.WriteSingle(Conversions.ToInteger(Operators.AddObject(objectValue, (pGlowIn * 0x38) + 4)), 4, col.r);
            Mem.WriteSingle(Conversions.ToInteger(Operators.AddObject(objectValue, (pGlowIn * 0x38) + 8)), 4, col.g);
            Mem.WriteSingle(Conversions.ToInteger(Operators.AddObject(objectValue, (pGlowIn * 0x38) + 12)), 4, col.b);
            Mem.WriteSingle(Conversions.ToInteger(Operators.AddObject(objectValue, (pGlowIn * 0x38) + 0x10)), 4, col.a);
            Mem.WriteBool(Conversions.ToInteger(Operators.AddObject(objectValue, (pGlowIn * 0x38) + 0x24)), 1, col.rwo);
            Mem.WriteBool(Conversions.ToInteger(Operators.AddObject(objectValue, (pGlowIn * 0x38) + 0x25)), 1, col.rwuo);
        }

        public static void PerformGlow()
        {
            while (MainPage.runCheat)
            {
                GlowStruct col = new GlowStruct
                {
                    r = (float)(((double)Config.glow_enemy_r) / 255.0),
                    g = (float)(((double)Config.glow_enemy_g) / 255.0),
                    b = (float)(((double)Config.glow_enemy_b) / 255.0),
                    a = (float)(((double)Config.glow_alpha) / 255.0),
                    rwo = true,
                    rwuo = false
                };
                GlowStruct struct3 = new GlowStruct
                {
                    r = (float)(((double)Config.glow_team_r) / 255.0),
                    g = (float)(((double)Config.glow_team_g) / 255.0),
                    b = (float)(((double)Config.glow_team_b) / 255.0),
                    a = (float)(((double)Config.glow_alpha) / 255.0),
                    rwo = true,
                    rwuo = false
                };
                if (Config.glowenemyenabled | Config.glowteamenabled)
                {
                    int num2 = 64;
                    int num = 1;
                    do
                    {
                        object objectValue = RuntimeHelpers.GetObjectValue(Mem.ReadInt(MainPage.clientBase + Offsets.OFSSET_LocalPlayer, 4));
                        object left = RuntimeHelpers.GetObjectValue(Mem.ReadInt((MainPage.clientBase + Offsets.OFSSET_EntityList) + ((num - 1) * 0x10), 4));
                        if (!left.Equals(0))
                        {
                            if (Conversions.ToBoolean(Operators.NotObject(RuntimeHelpers.GetObjectValue(Mem.ReadBool(Conversions.ToInteger(Operators.AddObject(left, Offsets.OFSSET_Dormant)), 4)))))
                            {
                                object obj5 = RuntimeHelpers.GetObjectValue(Mem.ReadInt(Conversions.ToInteger(Operators.AddObject(left, Offsets.OFSSET_GlowIndex)), 4));
                                object obj6 = RuntimeHelpers.GetObjectValue(Mem.ReadInt(Conversions.ToInteger(Operators.AddObject(left, Offsets.OFSSET_Team)), 4));
                                object right = RuntimeHelpers.GetObjectValue(Mem.ReadInt(Conversions.ToInteger(Operators.AddObject(objectValue, Offsets.OFSSET_Team)), 4));
                                if (Operators.ConditionalCompareObjectEqual(obj6, right, false))
                                {
                                    if (Config.glowteamenabled)
                                    {
                                        DrawGlow(Conversions.ToUInteger(obj5), struct3);
                                    }
                                }
                                else
                                {
                                    if (Config.glowenemyenabled)
                                    {
                                        DrawGlow(Conversions.ToUInteger(obj5), col);
                                    }
                                }
                            }                            
                        }
                        num++;
                    }
                    while (num <= num2);
                }
                System.Threading.Thread.Sleep(0x3);
            }
        }
    }
}
