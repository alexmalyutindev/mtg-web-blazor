precision mediump float;

attribute vec3 a_PositionOS;
attribute vec2 a_Texcoord;

uniform float u_Time;
uniform mat4 u_ObjectToWorld;
uniform mat4 u_WorldToView;
uniform mat4 u_Projection;

varying vec2 v_Texcoord;
varying vec3 v_PositionWS;

void main(void) {
    v_Texcoord = a_Texcoord;
    vec3 positionWS = (u_ObjectToWorld * vec4(a_PositionOS, 1.0)).xyz;
    vec3 positionVS = (u_WorldToView * vec4(positionWS, 1.0)).xyz;
    vec4 positionCS = u_Projection * vec4(positionVS, 1.0);

    v_PositionWS = positionWS;
    gl_Position = positionCS;
}