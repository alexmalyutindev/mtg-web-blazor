precision mediump float;
varying mediump vec2 v_Texcoord;

uniform sampler2D u_MainTex;

float checkers(in vec2 p)
{
    vec2 s = sign(fract(p*.5)-.5);
    return 0.5 - 0.5 * s.x * s.y;
}

void main(void) {
    vec2 uv = v_Texcoord.xy * 5.0;
    float checker = checkers(uv);
    gl_FragColor = mix(vec4(.2, .2, .2, 1.0), vec4(.9, .9, .9, 1.0), checker);
}