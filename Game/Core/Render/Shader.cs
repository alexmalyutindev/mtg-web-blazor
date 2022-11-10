using Blazor.Extensions.Canvas.WebGL;

namespace MtgWeb.Core.Render;

public class Shader
{
    public bool DepthTest = true;
    
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

    private static Dictionary<string, Shader> _shaders = new();


    public static async Task<Shader> Load(WebGLContext context, String name)
    {
        if (_shaders.TryGetValue(name, out var shader))
        {
            return shader;
        }

        var (vertex, fragment) = await Resources.LoadShader(name);
        shader = new Shader()
        {
            _vertexSrc = vertex,
            _fragmentSrc = fragment
        };

        await shader.Compile(context);
        await shader.LocateGlobalUniforms(context);

        // TODO: attributes & uniforms dictionary
        shader.PositionOS = (uint) await context.GetAttribLocationAsync(shader._program, "a_PositionOS");
        shader.Texcoord = (uint) await context.GetAttribLocationAsync(shader._program, "a_Texcoord");

        _shaders.Add(name, shader);

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