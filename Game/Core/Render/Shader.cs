using System.Runtime.CompilerServices;
using Blazor.Extensions.Canvas.WebGL;

namespace MtgWeb.Core.Render;

public class Shader
{
    public readonly string Name;
    public int Queue;

    public bool IsLoaded { get; private set; }
    public bool IsCompiled { get; private set; }

    public uint PositionOS { get; private set; }
    public uint Texcoord { get; private set; }

    public WebGLUniformLocation Time { get; private set; }
    public WebGLUniformLocation ObjectToWorld { get; private set; }
    public WebGLUniformLocation InvObjectToWorld { get; private set; }
    public WebGLUniformLocation WorldToView { get; private set; }
    public WebGLUniformLocation Projection { get; private set; }
    public WebGLUniformLocation CameraPositionWS { get; private set; }

    private WebGLProgram _program;
    private WebGLShader _vertexSader;
    private WebGLShader _fragmentShader;

    private String _vertexSrc;
    private String _fragmentSrc;

    private static readonly Dictionary<string, Shader> ShadersLibrary = new();

    private Shader(string name)
    {
        Name = name;
        ShadersLibrary.Add(Name, this);
    }

    public static Shader Create(string name)
    {
        if (ShadersLibrary.TryGetValue(name, out var shader))
        {
            return shader;
        }

        return new Shader(name);
    }

    public async Task Load()
    {
        var (vertex, fragment) = await Resources.LoadShader(Name);
        _vertexSrc = vertex;
        _fragmentSrc = fragment;

        IsLoaded = true;
    }
    
    public static async Task<Shader> Load(String name)
    {
        if (ShadersLibrary.TryGetValue(name, out var shader))
        {
            if (!shader.IsLoaded)
                shader.Load();
            return shader;
        }

        var (vertex, fragment) = await Resources.LoadShader(name);
        shader = new Shader(name)
        {
            _vertexSrc = vertex,
            _fragmentSrc = fragment,
            IsLoaded = true
        };

        return shader;
    }

    public static async Task CompileAll(WebGLContext context)
    {
        foreach (var shader in ShadersLibrary)
        {
            await CompileAndInit(context, shader.Value);
        }
    }

    private static async Task CompileAndInit(WebGLContext context, Shader shader)
    {
        if (!shader.IsLoaded)
        {
            await shader.Load();
        }

        if (!shader.IsCompiled)
        {
            var success = await shader.Compile(context);
            if (!success) return;
        }

        await shader.LocateGlobalUniforms(context);

        // TODO: attributes & uniforms dictionary
        shader.PositionOS = (uint) await context.GetAttribLocationAsync(shader._program, "a_PositionOS");
        shader.Texcoord = (uint) await context.GetAttribLocationAsync(shader._program, "a_Texcoord");

        shader.IsCompiled = true;
    }

    private async Task<bool> Compile(WebGLContext context)
    {
        Console.WriteLine($"[Shader] Compiling: {Name}");
        
        string error;

        _vertexSader = await context.CreateShaderAsync(ShaderType.VERTEX_SHADER)!;
        await context.ShaderSourceAsync(_vertexSader, _vertexSrc);
        await context.CompileShaderAsync(_vertexSader);

        error = await context.GetShaderInfoLogAsync(_vertexSader);
        if (!String.IsNullOrEmpty(error))
        {
            Console.WriteLine("Vertex:");
            Console.WriteLine(error);
        }

        _fragmentShader = await context.CreateShaderAsync(ShaderType.FRAGMENT_SHADER)!;
        await context.ShaderSourceAsync(_fragmentShader, _fragmentSrc);
        await context.CompileShaderAsync(_fragmentShader);

        error = await context.GetShaderInfoLogAsync(_fragmentShader);
        if (!String.IsNullOrEmpty(error))
        {
            Console.WriteLine("Fragment:");
            Console.WriteLine(error);
            return false;
        }

        _program = await context.CreateProgramAsync()!;
        await context.AttachShaderAsync(_program, _vertexSader);
        await context.AttachShaderAsync(_program, _fragmentShader);
        await context.LinkProgramAsync(_program);

        error = await context.GetProgramInfoLogAsync(_program);
        if (!String.IsNullOrEmpty(error))
        {
            Console.WriteLine(error);
            return false;
        }

        IsCompiled = true;
        return true;
    }

    private async Task LocateGlobalUniforms(WebGLContext context)
    {
        Console.WriteLine(nameof(LocateGlobalUniforms));
        
        Time = await context.GetUniformLocationAsync(_program, "u_Time");
        ObjectToWorld = await context.GetUniformLocationAsync(_program, "u_ObjectToWorld");
        InvObjectToWorld = await context.GetUniformLocationAsync(_program, "u_InvObjectToWorld");
        WorldToView = await context.GetUniformLocationAsync(_program, "u_WorldToView");
        Projection = await context.GetUniformLocationAsync(_program, "u_Projection");
        CameraPositionWS = await context.GetUniformLocationAsync(_program, "u_CameraPositionWS");
        
        Console.WriteLine(Time?.Id);
        Console.WriteLine(ObjectToWorld?.Id);
        Console.WriteLine(InvObjectToWorld?.Id);
        Console.WriteLine(WorldToView?.Id);
        Console.WriteLine(Projection?.Id);
        Console.WriteLine(CameraPositionWS?.Id);
    }

    public async Task Bind(WebGLContext context)
    {
        await context.UseProgramAsync(_program);
    }
}