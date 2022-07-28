struct UniformBuffer {
    mdlMat : mat4x4<f32>
};

struct VOut {
    @builtin(position) pos : vec4<f32>,
    @location(1) col : vec4<f32>,
    @location(2) uv : vec2<f32>
};

@group(0)
@binding(0)
var<uniform> ub : UniformBuffer;

@stage(vertex)
fn vs_main(@location(0) pos: vec3<f32>, @location(1) col: vec4<f32>, @location(2) uv: vec2<f32>) -> VOut {
    

    return VOut(ub.mdlMat*vec4<f32>(pos, 1.0), col, uv);
}



@group(0)
@binding(1)
var samp : sampler;

@group(0)
@binding(2)
var tex : texture_2d<f32>;

@stage(fragment)
fn fs_main(in : VOut) -> @location(0) vec4<f32> {
    let rpos = vec2<f32>(
        floor(in.uv.x*10.0),
        floor(in.uv.y*10.0)
    );

    let texCol = textureSample(tex,samp,in.uv);

    let col = mix(in.col, vec4<f32>(texCol.rgb,1.0), texCol.a);

    return col * mix(1.0, 0.9, f32((rpos.x%2.0+2.0)%2.0 == (rpos.y%2.0+2.0)%2.0));
}