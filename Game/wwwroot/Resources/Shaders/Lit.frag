precision mediump float;

// Lighting
uniform vec3 u_MainLightDirWS;
uniform vec3 u_MainLightColor;

// Varings
varying vec2 v_Texcoord;
varying vec3 v_NormalWS;

void main() {
    // TODO: Texture
    float NdotL = max(0.0, dot(v_NormalWS, u_MainLightDirWS));
    gl_FragColor = vec4(u_MainLightColor * NdotL, 1.0);
}