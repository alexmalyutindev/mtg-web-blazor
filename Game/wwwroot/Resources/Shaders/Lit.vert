precision mediump float;

// GLobal Uniforms
uniform float u_Time;
uniform mat4 u_ObjectToWorld;
uniform mat4 u_InvObjectToWorld;
uniform mat4 u_WorldToView;
uniform mat4 u_Projection;
uniform vec3 u_CameraPositionWS;

// Lighting
uniform vec3 u_MainLightDirWS;
uniform vec3 u_MainLightColor;

// Attributes
attribute vec3 a_PositionOS;
attribute vec3 a_NormalOS;
attribute vec2 a_Texcoord;

// Varings
varying vec2 v_Texcoord;
varying vec3 v_NormalWS;

void main() {
    vec3 positionWS = (u_ObjectToWorld * vec4(a_PositionOS, 1.0)).xyz;
    vec3 positionVS = (u_WorldToView * vec4(positionWS, 1.0)).xyz;
    vec4 positionCS = u_Projection * vec4(positionVS, 1.0);

    v_Texcoord = a_Texcoord;
    v_NormalWS = vec3((u_ObjectToWorld * vec4(a_NormalOS, 0.0)).xyz);
    
    gl_Position = positionCS;
}