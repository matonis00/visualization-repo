Shader "Unlit/GraphShader"
{
    Properties
    {
        _ScaleX ("Scale X", Float) = 16
        _ScaleY ("Scale Y", Float) = 16
        _OffsetX ("Offset X", Float) = -0.5
        _OffsetY ("Offset Y", Float) = -0.5
        _UnitPerGridX ("Unit per Grid on X" ,Float) = 1.0 
        _UnitPerGridY ("Unit per Grid on Y" ,Float) = 1.0 
        [Enum(Normal,1,AllwaysConnected,2,Catmull,3)] _LineVariant ("Line Variant", int) = 1
        _LineColor ("Line Color", Color) = (1,0,0,1)
        _LineSize ("Line Size", Float) = 0.05
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
            #pragma target 4.0
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
            

            int _MinPointsAmount;
            int _MaxPointsAmount;
            int _GraphsAmount;
            int _TexAmount;
            sampler1D _GraphsTex;

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

            float denormalization(float value, float minValue, float maxValue)
            {
                return float((value *(maxValue - minValue)) + minValue);
            }

            float2 normalization(float2 pDeNormalized, float minX, float maxX, float minY, float maxY)
            {
                return float2(((pDeNormalized.x - minX) / (maxX - minX)),((pDeNormalized.y - minY )/(maxY - minY) ));
            }

            float normalization(float value, float minValue, float maxValue)
            {
                return float((value - minValue) / (maxValue - minValue));
            }


            float drawCircle(float2 position, float2 uv, float size){
                return step(length(abs(position - uv)),  size );
            }
            
            float drawSquare(float2 position, float2 uv, float sizeX,float sizeY){
                return step(distance(position.x,uv.x), sizeX) * step(distance(position.y,uv.y), sizeY);
            }

            
            float Line(float2 p, float2 a, float2 b, float thickness) 
            {                
                float cy;
                if(a.x <= p.x && p.x <= b.x)
                {
                    float tmp = (p.x - a.x) / (b.x - a.x);
                    cy = a.y +  tmp * (b.y - a.y);
                    float res = smoothstep(0.0,  thickness , abs(p.y - cy));
                    return res;
                    
                }
                else
                {
                    return 1.0;
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

                
            }

            float Line3(float2 p, float2 a, float2 b, float thickness) 
            {
                

                float2 pa = p - a;
                float2 ba = b - a;

                float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
                float idk = length(pa - ba*h);

                return smoothstep(0.0, thickness, idk);
            }



            float2 splineInterpolation(float2 p0, float2 p1, float2 p2, float2 p3, float t) {
                float alpha = 1.0;
                float tension = 0.0;
                
                float t01 = pow(distance(p0, p1), alpha);
                float t12 = pow(distance(p1, p2), alpha);
                float t23 = pow(distance(p2, p3), alpha);

                float2 m1 = (1.0f - tension) *
                    (p2 - p1 + t12 * ((p1 - p0) / t01 - (p2 - p0) / (t01 + t12)));
                float2 m2 = (1.0f - tension) *
                    (p2 - p1 + t12 * ((p3 - p2) / t23 - (p3 - p1) / (t12 + t23)));
                
                float2 a = 2.0f * (p1 - p2) + m1 + m2;
                float2 b = -3.0f * (p1 - p2) - m1 - m1 - m2;
                float2 c = m1;
                float2 d = p1;

                return a * t * t * t +
                    b * t * t +
                    c * t +
                    d;

            }



            float Spline(float2 p, float2 p0, float2 p1,float2 p2,float2 p3, float thickness, int subLines) 
            {
                float curve = 0.0;
                float2 a = p1;

                for (int i = 1; i <= subLines; i++) {
                    float2 b = splineInterpolation(p0, p1, p2, p3, float(i)*(1.0/float(subLines)));
                    curve = Line3(p,a,b,thickness);
                    if(curve != 1.0) return 0.0;
                    a = b;
                }
                
                return curve;              
                
                
            }






            Interpolators vert (MeshData v)
            {
                Interpolators o;
                //Scale and Offset for uv coordinates
                float2 scale = float2(_ScaleX, _ScaleY);
                float2 offset = float2(_OffsetX, _OffsetY);
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                o.uv = (v.uv0 ) * scale + offset;
                o.normals = v.normals;
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                //Calculating axis
                float axisThickness = 0.1 ;//(4.0 /450.0) *16.0;
                float axis = min(abs(i.uv.x), abs(i.uv.y));
                axis = smoothstep(axisThickness -0.05, axisThickness,axis);
                
                //Calculating grid
                float2 temp = abs(i.uv % _UnitPerGridX); 
                float gridThickness = 0.04 *_UnitPerGridX;// (1.0/450.0) *16.0;
                float gridSize = 0.5 * _UnitPerGridX;
                float gridX = gridSize - abs(temp.x - gridSize);
                gridX = smoothstep(0.0,gridThickness,gridX);


                temp = abs(i.uv % _UnitPerGridY); 
                gridThickness = 0.04 * _UnitPerGridY;// (1.0/450.0) *16.0;
                gridSize = 0.5 * _UnitPerGridY;
                float gridY = gridSize -  abs(temp.y - gridSize);
                gridY = smoothstep(0.0,gridThickness,gridY);
                

                //Drawing points and Lines
                float4 finalPixelColor = float4(1,1,1,1);
                float isPoint = 0.0;
                float isLine = 1.0;
                float isColored = 0.0;
                float pointReversedAmount = 1.0/float(_TexAmount-1);
                int procesedIndex = 0;
                [loop]
                for(int j = 0; j < _GraphsAmount; ++j)
                {
                    
                    float4 pointColorOfGraph = tex1D(_GraphsTex, procesedIndex*pointReversedAmount);
                    procesedIndex++;
                    float4 lineColorOfGraph = tex1D(_GraphsTex, procesedIndex*pointReversedAmount);
                    procesedIndex++;
                    float numberOfPointsInGraphToNormlize = tex1D(_GraphsTex, procesedIndex*pointReversedAmount).x; // Getting amount of points in Graph
                    procesedIndex++;

                    float numberOfPointsInGraph = denormalization(numberOfPointsInGraphToNormlize, _MinPointsAmount,_MaxPointsAmount);
                    float2 beforePointValue;

                    if(_LineVariant == 3)
                    {
                        
                        int tmpProcesedIndex = procesedIndex;
                        [loop]
                        for(int pIndex = 0; pIndex < numberOfPointsInGraph; pIndex++, procesedIndex++)
                        {
                            float2 actualPointValue = tex1D(_GraphsTex, procesedIndex*pointReversedAmount).xy; // Getting point xy values
                            actualPointValue = denormalization(actualPointValue, _MinXValue, _MaxXValue, _MinYValue,_MaxYValue); // Denormalizing values of point
                            

                            //Checking if pixel is belong to point
                            if(_PointShape == 1)
                            isPoint = (isPoint != 0) ? isPoint : drawSquare(actualPointValue,i.uv,_PointSize ,_PointSize );
                            else if(_PointShape == 2)
                            isPoint = (isPoint != 0) ? isPoint : drawCircle(actualPointValue,i.uv,_PointSize);

                            if(isPoint != 0)
                            {
                                finalPixelColor = pointColorOfGraph;
                                isColored = 1.0;
                                return finalPixelColor;
                            }
                        }
                        procesedIndex = tmpProcesedIndex;

                        [loop]
                        for(int pIndex = 0; pIndex < numberOfPointsInGraph-1 ; pIndex++, procesedIndex++)
                        {
                            float2 p0;
                            float2 p3;
                            if(pIndex == 0)
                            {
                                p0 = float2(0.0,0.0); // Getting point xy values
                                //p0 = denormalization(p0, _MinXValue, _MaxXValue, _MinYValue,_MaxYValue);

                                p3 = tex1D(_GraphsTex, (procesedIndex+2)*pointReversedAmount).xy; // Getting point xy values
                                p3 = denormalization(p3, _MinXValue, _MaxXValue, _MinYValue,_MaxYValue);
                            }
                            else if(pIndex == (numberOfPointsInGraph - 2))
                            {
                                p0 = tex1D(_GraphsTex, (procesedIndex-1)*pointReversedAmount).xy; // Getting point xy values
                                p0 = denormalization(p0, _MinXValue, _MaxXValue, _MinYValue,_MaxYValue);

                                float p2X = tex1D(_GraphsTex, (procesedIndex+1)*pointReversedAmount).x;
                                p3 = float2(p2X,0.0); // Getting point xy values
                                p3 = denormalization(p3, _MinXValue, _MaxXValue, _MinYValue,_MaxYValue);
                                p3 = float2(p3.x+1.0,0.0);
                            }
                            else
                            {
                                p0 = tex1D(_GraphsTex, (procesedIndex-1)*pointReversedAmount).xy; // Getting point xy values
                                p0 = denormalization(p0, _MinXValue, _MaxXValue, _MinYValue,_MaxYValue);

                                p3 = tex1D(_GraphsTex, (procesedIndex+2)*pointReversedAmount).xy; // Getting point xy values
                                p3 = denormalization(p3, _MinXValue, _MaxXValue, _MinYValue,_MaxYValue);
                            }

                            float2 p1 = tex1D(_GraphsTex, (procesedIndex)*pointReversedAmount).xy; // Getting point xy values
                            p1 = denormalization(p1, _MinXValue, _MaxXValue, _MinYValue,_MaxYValue);
                            float2 p2 = tex1D(_GraphsTex, (procesedIndex+1)*pointReversedAmount).xy; // Getting point xy values
                            p2 = denormalization(p2, _MinXValue, _MaxXValue, _MinYValue,_MaxYValue);
                            
                            
                            isLine = Spline(i.uv,p0,p1,p2,p3,_LineSize,20);


                            if(isLine != 1.0)
                            {
                                finalPixelColor = lineColorOfGraph;
                                isColored = 1.0;
                                return finalPixelColor;
                            }
                            
                        }
                        procesedIndex++;
                        
                        
                    }
                    else
                    {
                        [loop]
                        for(int pIndex = 0; pIndex < numberOfPointsInGraph ; pIndex++, procesedIndex++)
                        {
                            float2 actualPointValue = tex1D(_GraphsTex, procesedIndex*pointReversedAmount).xy; // Getting point xy values
                            actualPointValue = denormalization(actualPointValue, _MinXValue, _MaxXValue, _MinYValue,_MaxYValue); // Denormalizing values of point
                            
                            //Checking if pixel is belong to point
                            if(_PointShape == 1)
                            isPoint = (isPoint != 0) ? isPoint : drawSquare(actualPointValue,i.uv,_PointSize,_PointSize);
                            else if(_PointShape == 2)
                            isPoint = (isPoint != 0) ? isPoint : drawCircle(actualPointValue,i.uv,_PointSize);

                            if(isPoint != 0)
                            {
                                finalPixelColor = pointColorOfGraph;
                                isColored = 1.0;
                                return finalPixelColor;
                            }


                            if(pIndex >= 0 && pIndex < (numberOfPointsInGraph-3) && _LineVariant == 3 && isColored != 1.0)
                            {
                                float2 p0 = tex1D(_GraphsTex, procesedIndex*pointReversedAmount).xy; // Getting point xy values
                                p0 = denormalization(p0, _MinXValue, _MaxXValue, _MinYValue,_MaxYValue);
                                float2 p1 = tex1D(_GraphsTex, (procesedIndex+1)*pointReversedAmount).xy; // Getting point xy values
                                p1 = denormalization(p1, _MinXValue, _MaxXValue, _MinYValue,_MaxYValue);
                                float2 p2 = tex1D(_GraphsTex, (procesedIndex+2)*pointReversedAmount).xy; // Getting point xy values
                                p2 = denormalization(p2, _MinXValue, _MaxXValue, _MinYValue,_MaxYValue);
                                float2 p3 = tex1D(_GraphsTex, (procesedIndex+3)*pointReversedAmount).xy; // Getting point xy values
                                p3 = denormalization(p3, _MinXValue, _MaxXValue, _MinYValue,_MaxYValue);
                                isLine = Spline(i.uv,p0,p1,p2,p3,_LineSize,100);
                                beforePointValue = actualPointValue;
                            }
                            else if(pIndex >= 1)
                            {   
                                //Checking if pixel is belong to line
                                if(_LineVariant == 1)
                                isLine = Line3(i.uv,beforePointValue,actualPointValue,_LineSize);
                                else if (_LineVariant == 2)
                                isLine = Line2(i.uv,beforePointValue,actualPointValue,_LineSize, isLine);
                                beforePointValue = actualPointValue;
                            }
                            else
                            {
                                beforePointValue = actualPointValue;
                            }

                            if(isLine != 1.0)
                            {
                                finalPixelColor = lineColorOfGraph;
                                isColored = 1.0;
                                return finalPixelColor;
                            }
                        }
                    }
                    
                }
                
                return  isColored == 1.0 ? finalPixelColor : float4(min(min(gridX,gridY), axis).xxx,1);
            }
            ENDCG
        }
    }
}
