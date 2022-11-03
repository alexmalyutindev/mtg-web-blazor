using Blazor.Extensions.Canvas.WebGL;

namespace MtgWeb.Core;

public class Shader
{
    public uint PositionOS { get; private set; }
    public uint Texcoord { get; private set; }
    public WebGLUniformLocation ObjectToWorld { get; private set; }
    public WebGLUniformLocation WorldToView { get; private set; }
    public WebGLUniformLocation Projection { get; private set; }

    private WebGLProgram _program;
    private WebGLShader _vertexSader;
    private WebGLShader _fragmentSader;


    public static async Task<Shader> CheckerShader(WebGLContext gl)
    {
        string vertCode = @"
        uniform float u_Time;
        uniform mat4 u_ObjectToWorld;
        uniform mat4 u_WorldToView;
        uniform mat4 u_Projection;

        attribute vec3 a_PositionOS;
        attribute vec2 a_Texcoord;

        varying vec2 v_Texcoord;

        void main(void) {
            v_Texcoord = a_Texcoord;
            vec3 positionWS = (u_ObjectToWorld * vec4(a_PositionOS, 1.0)).xyz;
            vec3 positionVS = (u_WorldToView * vec4(positionWS, 1.0)).xyz;

            vec4 positionCS = u_Projection * vec4(positionVS, 1.0);

            positionCS = vec4(positionWS, 1);
            gl_Position = positionCS;
        }";

        var fragCode = @"
        precision mediump float;
        varying mediump vec2 v_Texcoord;

        uniform sampler2D u_MainTex;

        float checkers(in vec2 p)
        {
            vec2 s = sign(fract(p*.5)-.5);
            return 0.5 - 0.5 * s.x * s.y;
        }

        void main(void) {
            float checker = checkers(v_Texcoord.xy * 5.0);
            gl_FragColor = mix(vec4(.2, .2, .2, 1.0), vec4(.9, .9, .9, 1.0), checker);
        }";

        string error;

        var vertShader = await gl.CreateShaderAsync(ShaderType.VERTEX_SHADER)!;
        await gl.ShaderSourceAsync(vertShader, vertCode);
        await gl.CompileShaderAsync(vertShader);

        error = await gl.GetShaderInfoLogAsync(vertShader);

        if (!String.IsNullOrEmpty(error))
            Console.WriteLine(error);

        var fragmentShader = await gl.CreateShaderAsync(ShaderType.FRAGMENT_SHADER)!;
        await gl.ShaderSourceAsync(fragmentShader, fragCode);
        await gl.CompileShaderAsync(fragmentShader);

        error = await gl.GetShaderInfoLogAsync(fragmentShader);
        if (!String.IsNullOrEmpty(error))
            Console.WriteLine(error);

        var program = await gl.CreateProgramAsync()!;
        await gl.AttachShaderAsync(program, vertShader);
        await gl.AttachShaderAsync(program, fragmentShader);
        await gl.LinkProgramAsync(program);

        error = await gl.GetProgramInfoLogAsync(program);
        if (!String.IsNullOrEmpty(error))
            Console.WriteLine(error);
        
        var positionOS = await gl.GetAttribLocationAsync(program, "a_PositionOS");
        var texcoord = await gl.GetAttribLocationAsync(program, "a_Texcoord");

        var objectToWorld = await gl.GetUniformLocationAsync(program, "u_ObjectToWorld");
        
        return new Shader()
        {
            ObjectToWorld = objectToWorld,
            PositionOS = (uint) positionOS,
            Texcoord = (uint) texcoord,
            _vertexSader = vertShader,
            _fragmentSader = fragmentShader,
            _program = program
        };
    }

    public async Task Bind(WebGLContext context)
    {
        await context.UseProgramAsync(_program);
    }
}