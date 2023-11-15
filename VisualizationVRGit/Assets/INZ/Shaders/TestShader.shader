Shader "Unlit/TestShader"
{
    Properties
    {
        _ScaleX ("Scale X", Float) = 16
        _ScaleY ("Scale Y", Float) = 16
        _OffsetX ("Offset X", Float) = -0.5
        _OffsetY ("Offset Y", Float) = -0.5
        _UnitPerGridX ("Unit per Grid on X" ,Float) = 1.0 
        _UnitPerGridY ("Unit per Grid on Y" ,Float) = 1.0 
        //[KeywordEnum(Normal,AllwaysConnected)] _LineVariant ("Line Variant", int) = 0
        [Enum(Normal,1,AllwaysConnected,2)] _LineVariant ("Line Variant", int) = 1
        _LineColor ("Line Color", Color) = (1,0,0,1)
        _LineSize ("Line Size", Float) = 0.05
        //[KeywordEnum(Square,Circle)] _PointShape ("Point Shape", int) = 0
        [Enum(Square,1,Circle,2)] _PointShape ("Point Shape", int) = 1
        _PointColor ("Point Color", Color) = (0,0,0,1)
        _PointSize ("Point Size", Float) = 0.5
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _LINEVARIANT_NORMAL _LINEVARIANT_ALLWAYSCONNECTED
            #pragma shader_feature _POINTSHAPE_SQUARE _POINTSHAPE_CIRCLE 
            #include "UnityCG.cginc"

            #define TAU 6.28318530718

            float4 _PointColor;
            float _PointSize;
            int _PointShape;
            
            float4 _LineColor;
            float _LineSize;
            int _LineVariant;

            float _ScaleX; 
            float _ScaleY;
            float _OffsetX;
            float _OffsetY;

            float _UnitPerGridX;
            float _UnitPerGridY;
            float _MinXValue;
            float _MaxXValue;
            float _MinYValue;
            float _MaxYValue;
            

            
            float4 _Points[10];
            int _PointsAmount;

            sampler1D _ArrayTex;

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normals : NORMAL;
                float4 tangent : TANGENT;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normals : TEXCOORD1;
                float4 tangent : TEXCOORD2;
            };

            float InverseLerp(float a, float b, float v)
            {
                return (v-a)/(b-a);
            }

            float2 denormalization(float2 pNormalized, float minX, float maxX, float minY, float maxY)
            {
                return float2(((pNormalized.x *(maxX - minX)) + minX),((pNormalized.y *(maxY - minY)) + minY));
            }

            float2 normalization(float2 pDeNormalized, float minX, float maxX, float minY, float maxY)
            {
                return float2(((pDeNormalized.x - minX) / (maxX - minX)),((pDeNormalized.y - minY )/(maxY - minY) ));
            }

            float drawCircle(float2 position, float2 uv, float size){
                return step(length(abs(position - uv)),  size );
            }
            
            float drawSquare(float2 position, float2 uv, float size){
                return step(distance(position.x,uv.x), size) * step(distance(position.y,uv.y), size);
            }

            
            float Line(float2 p, float2 a, float2 b, float thickness,float previos) 
            {
                
                //float tmp = (p.x - a.x) / (b.x - a.x);
                //float cy = a.y +  tmp * (b.y - a.y);
                //return smoothstep(0.0,  thickness , abs(p.y - cy));

                float cy;
                if(previos != 1.0) return previos;
                else if(a.x <= p.x && p.x <= b.x)
                {
                    float tmp = (p.x - a.x) / (b.x - a.x);
                    cy = a.y +  tmp * (b.y - a.y);
                    float res = smoothstep(0.0,  thickness , abs(p.y - cy));
                    return res;
                    
                }
                else
                {
                    return previos;
                }


            }

            float Line2(float2 p, float2 a, float2 b, float thickness, float previos) 
            {
                float cy;
                if(previos != 1.0) return previos;
                else if(a.x <= p.x && p.x <= b.x)
                {
                    float tmp = (p.x - a.x) / (b.x - a.x);
                    cy = a.y +  tmp * (b.y - a.y);
                    float res = smoothstep(0.0,  thickness , abs(p.y - cy));
                    return res;
                    
                }
                else if(a.y <= p.y && p.y <= b.y)
                {
                    float tmp =  (p.y - a.y) / (b.y - a.y);
                    cy =  a.x +  tmp * (b.x - a.x);
                    float res = smoothstep(0.0,  thickness , abs(p.x - cy));
                    return res;
                }
                else
                {
                    return previos;
                }
                
                //float xd = acos( dot(a,b) / (distance(a,float2(0,0)) * distance(b,float2(0,0))));
                
            }

            float Line2(float2 p, float2 a, float2 b, float thickness) 
            {
                float sine = sin(((p.x - a.x) / (b.x - a.x) * 2.0 - 1.0) * 1.5707963267);
                float cy = a.y + (0.5 + 0.5 * sine) * (b.y - a.y);
                return smoothstep(0.0, thickness, abs(p.y - cy));
            }
                     

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                //Scale and Offset for uv coordinates
                float2 scale = float2(_ScaleX, _ScaleY);
                float2 offset = float2(_OffsetX, _OffsetY);
                //_PointA = (_PointA + offset) * scale;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                o.uv = (v.uv0 ) * scale + offset;
                o.normals = v.normals;
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                //Calculating axis
                float axisThickness = 0.1422;//(4.0 /450.0) *16.0;
                float axis = min(abs(i.uv.x), abs(i.uv.y));
                axis = smoothstep(axisThickness -0.05, axisThickness,axis);
                
                //Calculating grid
                float2 temp = abs(i.uv % _UnitPerGridX); // modulo co ile ma występowac kratka ->  co jedną jednostkę
                float gridThickness = 0.0355;// (1.0/450.0) *16.0;
                float gridSize = 0.5 * _UnitPerGridX;
                float gridX = gridSize - abs(temp.x - gridSize);
                //float grid = gridSize - max(abs(temp.x - gridSize), abs(temp.y - gridSize));
                gridX = smoothstep(0.0,gridThickness,gridX);



                temp = abs(i.uv % _UnitPerGridY); // modulo co ile ma występowac kratka ->  co jedną jednostkę
                gridThickness = 0.0355;// (1.0/450.0) *16.0;
                gridSize = 0.5 * _UnitPerGridY;
                float gridY = gridSize -  abs(temp.y - gridSize);
                gridY = smoothstep(0.0,gridThickness,gridY);

                //Scalling point to graph scale and offset
                //float2 scale = float2(_ScaleX, _ScaleY);
                //float2 offset = float2(_OffsetX, _OffsetY);
                //_PointA = (_PointA + offset) * scale;
                
                //Drawing points
                float isPoint = 0.0;
                float pointReversedAmount = 1.0/float(_PointsAmount-1);
                for(int j = 0; j<_PointsAmount; ++j)
                {
                    float2 pvalue = tex1D(_ArrayTex, j*pointReversedAmount).xy; // Getting point xy values
                    pvalue = denormalization(pvalue, _MinXValue, _MaxXValue, _MinYValue,_MaxYValue); // Denormalizing values of point
                    if(_PointShape == 1)
                    isPoint = (isPoint != 0) ? isPoint : drawSquare(pvalue,i.uv,_PointSize);
                    else if(_PointShape == 2)
                    isPoint = (isPoint != 0) ? isPoint : drawCircle(pvalue,i.uv,_PointSize);
                    
                }
                

                float2 pointBefore = denormalization(tex1D(_ArrayTex, 0*pointReversedAmount).xy, _MinXValue, _MaxXValue, _MinYValue,_MaxYValue);
                float2 pointAfter;
                float isLine = 1.0;
                for(int h = 1; h<_PointsAmount; ++h)
                {
                    pointAfter = denormalization(tex1D(_ArrayTex, h*pointReversedAmount).xy, _MinXValue, _MaxXValue, _MinYValue,_MaxYValue);
                    //isLine = (pointBefore.x <= i.uv.x && i.uv.x <= pointAfter.x) ? Line(i.uv,pointBefore,pointAfter,_LineSize) : (isLine != 1.0)? isLine : 1.0;
                    //isLine = (pointBefore.y <= i.uv.y && i.uv.y <= pointAfter.y) ?Line3(i.uv,pointBefore,pointAfter,_LineSize) : (pointBefore.x <= i.uv.x && i.uv.x <= pointAfter.x) ? Line(i.uv,pointBefore,pointAfter,_LineSize) : (isLine != 1.0)? isLine : 1.0;
                    if(_LineVariant == 1)
                    isLine = Line(i.uv,pointBefore,pointAfter,_LineSize, isLine);
                    else if (_LineVariant == 2)
                    isLine = Line2(i.uv,pointBefore,pointAfter,_LineSize, isLine);
                    pointBefore = pointAfter;
                }

                return  isPoint == 1.0 ? _PointColor : (isLine != 1.0)? _LineColor :  float4(min(min(gridX,gridY), axis).xxx,1);
            }
            ENDCG
        }
    }
}
