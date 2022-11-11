#version 120

uniform float u_Time;
uniform mat4 u_ObjectToWorld;
uniform mat4 u_InvObjectToWorld;
uniform mat4 u_WorldToView;
uniform mat4 u_Projection;

uniform vec3 u_CameraPostionWS;

attribute vec3 a_PositionOS;
attribute vec2 a_Texcoord;

varying vec2 v_Texcoord;
varying vec3 v_PositionWS;
varying vec3 v_ViewDirWS;
varying vec3 v_ViewDirOS;

void main() {
    v_Texcoord = a_Texcoord;
    v_PositionOS = a_PositionOS;
    
    vec3 positionWS = (u_ObjectToWorld * vec4(a_PositionOS, 1.0)).xyz;
    vec3 positionVS = (u_WorldToView * vec4(positionWS, 1.0)).xyz;
    vec4 positionCS = u_Projection * vec4(positionVS, 1.0);

    v_ViewDirWS = normalize(u_CameraPostionWS - positionWS);
    v_ViewDirOS = mul(u_InvObjectToWorld, vec4(v_ViewDirWS, 0.0));

    gl_Position = positionCS;
}
