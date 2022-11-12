precision mediump float;

attribute vec3 a_PositionOS;
attribute vec2 a_Texcoord;

uniform float u_Time;
uniform mat4 u_ObjectToWorld;
uniform mat4 u_InvObjectToWorld;
uniform mat4 u_WorldToView;
uniform mat4 u_Projection;
uniform vec3 u_CameraPositionWS;

varying vec3 v_PositionOS;
varying vec3 v_PositionVS;
varying vec3 v_PositionWS;
varying vec3 v_RayOrigin;
varying vec3 v_RayDir;

void main() {
    v_PositionOS = a_PositionOS;

    vec3 positionWS = (u_ObjectToWorld * vec4(a_PositionOS, 1.0)).xyz;
    vec3 positionVS = (u_WorldToView * vec4(positionWS, 1.0)).xyz;
    vec4 positionCS = u_Projection * vec4(positionVS, 1.0);

    v_PositionWS = positionWS;
    v_PositionVS = positionVS;

    vec3 viewDirWS = normalize(u_CameraPositionWS - positionWS);
    v_RayDir = (u_InvObjectToWorld * vec4(viewDirWS, 0.0)).xyz;
    v_RayOrigin = (u_InvObjectToWorld * vec4(u_CameraPositionWS, 1.0)).xyz;

    gl_Position = positionCS;
}
