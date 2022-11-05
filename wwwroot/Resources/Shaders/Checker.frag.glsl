precision mediump float;
precision mediump vec2;

varying vec2 v_Texcoord;

uniform sampler2D u_MainTex;

float checkers(in vec2 p)
{
    vec2 s = sign(fract(p*.5)-.5);
    return 0.5 - 0.5 * s.x * s.y;
}

// --- analytically box-filtered checkerboard ---
float checkersTextureGradBox(in vec2 p, in vec2 ddx, in vec2 ddy)
{
    // filter kernel
    vec2 w = max(abs(ddx), abs(ddy)) + 0.01;
    // analytical integral (box filter)
    vec2 i = 2.0*(abs(fract((p-0.5*w)/2.0)-0.5)-abs(fract((p+0.5*w)/2.0)-0.5))/w;
    // xor pattern
    return 0.5 - 0.5*i.x*i.y;
}

void main(void) {
    vec2 ddx = dFdx(v_Texcoord.xy);
    vec2 ddy = dFdy(v_Texcoord.xy);

    float checker = checkersTextureGradBox(v_Texcoord, ddx, ddy); // checkers(v_Texcoord.xy * 5.0);
    gl_FragColor = mix(vec4(.2, .2, .2, 1.0), vec4(.9, .9, .9, 1.0), checker);
}