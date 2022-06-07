struct UniformBuffer {
    size : f32
};

struct VOut {
    @builtin(position) pos : vec4<f32>,
    @location(1) col : vec4<f32>
};

@group(0)
@binding(0)
var<uniform> ub : UniformBuffer;

//array didn't work for some reason
fn vtx_col(id: u32) -> vec4<f32>{
    switch(id) {
        case 0:  { return vec4<f32>(1.0,1.0,0.0,1.0); }
        case 1:  { return vec4<f32>(0.0,1.0,1.0,1.0); }
        case 2:  { return vec4<f32>(1.0,0.0,1.0,1.0); }
        default: { return vec4<f32>(0.0); }
    }
}

@stage(vertex)
fn vs_main(@location(0) pos: vec3<f32>, @location(1) col: vec4<f32>) -> VOut {
    

    return VOut(vec4<f32>(pos*ub.size, 1.0), col);
}

@stage(fragment)
fn fs_main(in : VOut) -> @location(0) vec4<f32> {
    let rpos = vec2<f32>(
        floor(in.pos.x*0.05),
        floor(in.pos.y*0.05)
    );
    return in.col * mix(f32(rpos.x%2.0 == rpos.y%2.0), 1.0, 0.9);
}