precision mediump float;

varying mediump vec2 v_Texcoord;
varying mediump vec3 v_PositionWS;

uniform mat4 u_ObjectToWorld;
uniform sampler2D u_MainTex;

float checkers(in vec2 p)
{
    vec2 q = floor(p);
    return mod(q.x+q.y,2.);
}

void main(void) {
    vec2 scaleXY = vec2(
        length(u_ObjectToWorld[0].xyz),
        length(u_ObjectToWorld[1].xyz)
    );
    vec2 uv = v_Texcoord.xy * scaleXY * 2.0;
    float checker = checkers(uv + vec2(0.5));
    gl_FragColor = mix(vec4(.2, .2, .2, 1.0), vec4(.9, .9, .9, 1.0), checker);
}