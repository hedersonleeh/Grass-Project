
#define PI  3.1416
#define TAU 6.28318530718

float2 random2(float2 st){
    st = float2( dot(st,float2(127.1,311.7)),
    dot(st,float2(269.5,183.3)) );
    return -1.0 + 2.0*frac(sin(st)*43758.5453123);
}
float random (float2 st) {
    return frac(sin(dot(st.xy,
    float2(12.9898,78.233)))
    * 43758.5453123);
}

float noise1D(in float st)
{
    float i = floor(st);
    float f = frac(st);
    st = lerp(random(i),random(i + 1),smoothstep(0.001,1.0,f));
    return  st;
}
float noise2D(in float2 st)
{
    float2 i = floor(st);
    float2 f = frac(st);

    float a = random(i);
    float b = random(i + float2(1.0, 0.0));
    float c = random(i + float2(0.0, 1.0));
    float d = random(i + float2(1.0, 1.0));

    float2 u = f*f*(3.0-2.0*f);

    return lerp(a, b, u.x) +
    (c - a)* u.y * (1.0 - u.x) +
    (d - b) * u.x * u.y;
}

    float2 RotateUV(float2 uvs,float angle)
            {

                float sinX = sin (angle );
                float cosX = cos (angle );
                float sinY = sin (angle );
                float2x2 rotationMatrix = float2x2( cosX, -sinX, sinY, cosX);
                return  uvs.xy = mul (uvs, rotationMatrix );
            }

float perlinNoise(float2 uv)
{
    float2 lv =frac(uv );
    float2 id = floor(uv );
    
    lv = lv * lv *(lv*(lv*6.-15)+10);//x*x*x*(x*(x*6.-15.)+10.)
    float bl = random(id); // 0,0
    float br = random(id + float2(1,0)); // 1,0

    float b = lerp(bl,br,lv.x);


    float tl = random(id+float2(0,1)); // 0,1
    float tr = random  (id + float2(1,1)); // 1,1

    float t = lerp(tl,tr,lv.x);
    float tb = lerp(b,t,lv.y);

    return tb;
}
float smoothNoise(float2 uv,float value)
{
    float n = perlinNoise(uv*4);
    n += perlinNoise(uv*8 )* .5;
    n += perlinNoise(uv*16 ) * .25;
    n += perlinNoise(uv *32 ) * .125;
    n += perlinNoise(uv *64 )*.0625;
    n += perlinNoise(uv *128 )*.03125;

    return n /= value;
}
float Voronoi(float2 uv)
{
    float minDist = 100;
    uv *= 10 ;

    float2 gv = frac(uv)-.5 ;
    float2 id = floor(uv)   ;
    float t = _Time.y * 2;
    
    for(float y=-1;y<= 1. ; y++)
        for(float x=-1;x<= 1. ; x++)
        {
            float2 offs = float2(x,y);

            float2 n = noise2D(id+ offs);
            float2 p = offs + sin(n *t )* .5;

             float d = length(gv - p);

             if(d < minDist)
             {
                 minDist = d;
             }
        }
    return minDist;
}
float Voronoi(float2 uv,float minDist)
{
    
    uv *= 10 ;

    float2 gv = frac(uv)-.5 ;
    float2 id = floor(uv)   ;
    float t = _Time.y;
    
    for(float y=-1;y<= 1. ; y++)
        for(float x=-1;x<= 1. ; x++)
        {
            float2 offs = float2(x,y);

            float2 n = noise2D(id+ offs);
            float2 p = offs + sin(n *t )* .5;

             float d = length(gv - p);

             if(d < minDist)
             {
                 minDist = d;
             }
        }
    return minDist;
}
float Voronoi(float2 uv,float minDist,out float2 cid)
{
    
    uv *= 3 ;

    float2 gv = frac(uv)-.5 ;
    float2 id = floor(uv)   ;
    float t = _Time.y ;
    
    for(float y=-1;y<= 1. ; y++)
        for(float x=-1;x<= 1. ; x++)
        {
            float2 offs = float2(x,y);

            float2 n = noise2D(id+ offs);
            float2 p = offs + sin(n *t )* .5;

             float d = length(gv - p) ;

             if(d < minDist)
             {
                 minDist = d;
                 cid = id + offs;
             }
        }
    return minDist;
}
