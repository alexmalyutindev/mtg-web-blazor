using Blazor.Extensions.Canvas.WebGL;

namespace MtgWeb.Core.Render;

public class Shader
{
    public uint PositionOS { get; private set; }
    public uint Texcoord { get; private set; }

    public WebGLUniformLocation Time { get; private set; }
    public WebGLUniformLocation ObjectToWorld { get; private set; }
    public WebGLUniformLocation WorldToView { get; private set; }
    public WebGLUniformLocation Projection { get; private set; }

    private WebGLProgram _program;
    private WebGLShader _vertexSader;
    private WebGLShader _fragmentShader;

    private String _vertexSrc;
    private String _fragmentSrc;


    public static async Task<Shader> CheckerShader(WebGLContext context)
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

            gl_Position = positionCS;
        }";

        var fragCode = @"
        precision mediump float;
        varying mediump vec2 v_Texcoord;

        uniform sampler2D u_MainTex;

        float checkers2(in vec2 p)
        {
            vec2 s = sign(fract(p*.5)-.5);
            return 0.5 - 0.5 * s.x * s.y;
        }

        float checkers( in vec2 p )
        {
            vec2 q = floor(p);
            return mod(q.x+q.y,2.);
        }

        void main(void) {
            float checker = checkers(v_Texcoord.xy * 5.0);
            gl_FragColor = mix(vec4(.2, .2, .2, 1.0), vec4(.9, .9, .9, 1.0), checker);
        }";

        // TODO: Shader loading from file.
        var shader = new Shader()
        {
            _vertexSrc = vertCode,
            _fragmentSrc = fragCode
        };

        await shader.Compile(context);
        await shader.LocateGlobalUniforms(context);

        // TODO: attributes & uniforms dictionary
        shader.PositionOS = (uint) await context.GetAttribLocationAsync(shader._program, "a_PositionOS");
        shader.Texcoord = (uint) await context.GetAttribLocationAsync(shader._program, "a_Texcoord");

        Console.WriteLine(
            $"ObjectToWorld: {shader.ObjectToWorld.Id}\n" +
            $"WorldToView: {shader.WorldToView.Id}\n" +
            $"Projection: {shader.Projection.Id}\n" +
            $"PositionOS: {shader.PositionOS}\n" +
            $"Texcoord: {shader.Texcoord}"
        );

        return shader;
    }

    public async Task Compile(WebGLContext context)
    {
        string error;

        _vertexSader = await context.CreateShaderAsync(ShaderType.VERTEX_SHADER)!;
        await context.ShaderSourceAsync(_vertexSader, _vertexSrc);
        await context.CompileShaderAsync(_vertexSader);

        error = await context.GetShaderInfoLogAsync(_vertexSader);
        if (!String.IsNullOrEmpty(error))
            Console.WriteLine(error);

        _fragmentShader = await context.CreateShaderAsync(ShaderType.FRAGMENT_SHADER)!;
        await context.ShaderSourceAsync(_fragmentShader, _fragmentSrc);
        await context.CompileShaderAsync(_fragmentShader);

        error = await context.GetShaderInfoLogAsync(_fragmentShader);
        if (!String.IsNullOrEmpty(error))
            Console.WriteLine(error);

        _program = await context.CreateProgramAsync()!;
        await context.AttachShaderAsync(_program, _vertexSader);
        await context.AttachShaderAsync(_program, _fragmentShader);
        await context.LinkProgramAsync(_program);

        error = await context.GetProgramInfoLogAsync(_program);
        if (!String.IsNullOrEmpty(error))
            Console.WriteLine(error);
    }

    private async Task LocateGlobalUniforms(WebGLContext context)
    {
        Time = await context.GetUniformLocationAsync(_program, "u_Time");
        ObjectToWorld = await context.GetUniformLocationAsync(_program, "u_ObjectToWorld");
        WorldToView = await context.GetUniformLocationAsync(_program, "u_WorldToView");
        Projection = await context.GetUniformLocationAsync(_program, "u_Projection");
    }

    public async Task Bind(WebGLContext context)
    {
        await context.UseProgramAsync(_program);
    }
}