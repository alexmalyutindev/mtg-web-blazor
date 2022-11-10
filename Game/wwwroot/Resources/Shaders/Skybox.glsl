#version 120

#pragma vertex Vertex
#pragma fragment Fragment

#include "./CoreUniforms.glsl"

attribute vec3 a_PositionOS;

varying vec3 v_ViewDirWS;

void Vertex() {
    v_ViewDirWS = (u_WorldToView * vec4(a_PositionOS, 1.0)).xyz;
    gl_Position = a_PositionOS;
}

void Vertex() {

}