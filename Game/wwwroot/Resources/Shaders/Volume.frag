precision mediump float;

uniform float u_Time;
uniform mat4 u_ObjectToWorld;
uniform mat4 u_InvObjectToWorld;
uniform mat4 u_WorldToView;
uniform mat4 u_Projection;
uniform vec3 u_CameraPostionWS;

uniform vec3 u_MainLightDir;

varying vec2 v_Texcoord;
varying vec3 v_PositionWS;
varying vec3 v_PositionOS;
varying vec3 v_ViewDirWS;
varying vec3 v_ViewDirOS;

vec2 BoxIntersection(in vec3 ro, in vec3 rd, in vec3 rad, in float depth)
{
    vec3 m = 1.0 / rd;
    vec3 n = m * ro;
    vec3 k = abs(m) * rad;
    vec3 t1 = -n - k;
    vec3 t2 = -n + k;

    float tN = max(max(t1.x, t1.y), t1.z);
    float tF = min(min(t2.x, t2.y), t2.z);

    // not visible (behind camera or behind dbuffer)
    if (tF < 0.0 || tN > depth) return vec2(-1.0);

    // clip integration segment from camera to dbuffer
    tN = max(tN, 0.0);
    tF = min(tF, depth);

    return vec2(tN, tF);
}

float maxcomp(in vec3 p) { return max(p.x, max(p.y, p.z));}

float boxDistance(in vec3 p, in vec3 rad)
{
    vec3 d = abs(p) - rad;
    return length(max(d, 0.0)) + min(maxcomp(d), 0.0);
}

float Sample(vec3 p) {
    float d = boxDistance(p, vec3(0.5));
    return clamp(-d, 0.0, 1.0);
}

void main() {
    const int steps = 5;
    float stepSize = 1.0 / float(steps);

    vec3 rayOrigin = v_PositionOS;
    vec3 rayDirection = v_ViewDirOS;

    vec3 lightRay = vec3(0);
    vec3 lightDir = -normalize(vec3(1, 1, 1)); // TODO: u_MainLightDir;
    lightDir *= stepSize;

    vec2 boxIntersection = BoxIntersection(rayOrigin, rayDirection, vec3(0.5), 10.0);
    rayDirection *= (boxIntersection.y - boxIntersection.x) * stepSize;

    //Settings 
    float stepDensity = 5.0 * stepSize;
    float shadowDensity = 10.0 * stepSize;
    
    float currentDensity = 0.0;
    float lightEnergy = 0.0;
    float transmittance = 1.0;
    float dencity = 0.0;

    float shadowDist = 0.0;
    float lightSample = 0.0;
    
    for (int i = 0; i < steps; i++) {
        dencity = Sample(rayOrigin);

        if (dencity > 0.001)
        { 
            lightRay = rayOrigin;
            shadowDist = 0.0;
            for (int l = 0; l < 5; l++) {
                lightRay += lightDir;

                lightSample = Sample(lightRay);
                shadowDist += lightSample;
            }
        }
        currentDensity = clamp(dencity * stepDensity, 0.0, 1.0);
        float shadowTerm = exp(-shadowDist * shadowDensity);
        float absorbedLight = shadowTerm * currentDensity;
        lightEnergy += absorbedLight * transmittance;
        transmittance *= 1.0 - currentDensity;

        rayOrigin += rayDirection;
    }

    gl_FragColor = vec4(lightEnergy, lightEnergy, lightEnergy, 1.0 - transmittance);
}
