using Blazor.Extensions.Canvas.WebGL;

namespace MtgWeb.Core;

public class Mesh
{
    public float[] Vertices;
    public float[] UVs;
    public int[] Indices;

    private WebGLBuffer _verticesBuffer;
    private WebGLBuffer _uvsBuffer;
    private WebGLBuffer _indicesBuffer;

    public static Mesh Quad()
    {
        var quad = new Mesh();

        quad.Vertices = new float[]
        {
            -0.5f, 0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f,
            0.5f, -0.5f, 0.0f,
            0.5f, 0.5f, 0.0f
        };
        quad.Indices = new[]
        {
            3, 2, 1,
            3, 1, 0
        };
        quad.UVs = new[]
        {
            0.0f, 1.0f,
            0.0f, 0.0f,
            1.0f, 0.0f,
            1.0f, 1.0f
        };
        return quad;
    }
    
    public async Task Init(WebGLContext context) {
        _verticesBuffer = await context.CreateBufferAsync();
        
        await context.BindBufferAsync(BufferType.ARRAY_BUFFER, _verticesBuffer);
        await context.BufferDataAsync(BufferType.ARRAY_BUFFER, Vertices, BufferUsageHint.STATIC_DRAW);

        _uvsBuffer = await context.CreateBufferAsync();
        await context.BindBufferAsync(BufferType.ARRAY_BUFFER, _uvsBuffer);
        await context.BufferDataAsync(BufferType.ARRAY_BUFFER, UVs, BufferUsageHint.STATIC_DRAW);

        _indicesBuffer = await context.CreateBufferAsync();
        await context.BindBufferAsync(BufferType.ELEMENT_ARRAY_BUFFER, _indicesBuffer);
        await context.BufferDataAsync(BufferType.ELEMENT_ARRAY_BUFFER, Indices, BufferUsageHint.STATIC_DRAW);
    }
}