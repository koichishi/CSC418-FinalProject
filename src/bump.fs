// Set the pixel color using Blinn-Phong shading (e.g., with constant blue and
// gray material color) with a bumpy texture.
// 
// Uniforms:
uniform mat4 view;
uniform mat4 proj;
uniform float animation_seconds;
uniform bool is_moon;
uniform bool is_boom;
// Inputs:
//                     linearly interpolated from tessellation evaluation shader
//                     output
in vec3 sphere_fs_in;
in vec3 normal_fs_in;
in vec4 pos_fs_in; 
in vec4 view_pos_fs_in; 
// Outputs:
//               rgb color of this pixel
out vec3 color;
// expects: model, blinn_phong, bump_height, bump_position,
// improved_perlin_noise, tangent

void main()
{  
  //   n  unit surface normal direction
  //   v  unit direction from point on object to eye
  //   l  unit light direction
    // Replace with your code
  //   n  unit surface normal direction
  //   v  unit direction from point on object to eye
  //   l  unit light direction
  vec3 ka = vec3(0.0);
  vec3 kd = vec3(0.8);
  if (is_moon) {
    kd = vec3(0.5);
  } 
  else {
    kd = vec3(0.1 ,0.1 ,1.0);
  }
  vec3 ks = vec3(0.8);

  vec3 light_direction = vec3(5, 5, 0);
  float theta = animation_seconds * M_PI / 4.0;
  mat4 rotation = mat4(cos(theta), 0.0, sin(theta), 0.0,
                       0.0       , 1.0, 0.0       , 0.0,
                      -sin(theta), 0.0, cos(theta), 0.0,
                       0.0       , 0.0, 0.0       , 1.0);
  vec3 l = normalize((view * rotation * vec4(1, 1, 1, 0)).xyz);

  float noise = improved_perlin_noise(sphere_fs_in);
  vec3 bump_sphere = bump_position(is_moon, sphere_fs_in);
  vec4 bump_view = view * model(is_moon, is_boom, -animation_seconds / 4.0) * vec4(bump_sphere, 1.0);

  vec3 v = normalize((bump_view).xyz);

  // calculate tangent (from tangent file)
  vec3 T, B;
  T = normalize(cross(vec3(1, 0, 0), sphere_fs_in)) == vec3(0) ? 
    normalize(cross(vec3(0, 1, 0), sphere_fs_in)) : normalize(cross(vec3(1, 0, 0), sphere_fs_in));
  B = normalize(cross(sphere_fs_in, T));

  float epsilon = 0.0001;
  vec3 temp1 = (bump_position(is_moon, sphere_fs_in + epsilon * T) - bump_sphere) / epsilon;
  vec3 temp2 = (bump_position(is_moon, sphere_fs_in + epsilon * B) - bump_sphere) / epsilon;
  vec3 bump_normal = normalize(cross(temp1, temp2));

  bump_normal = dot(sphere_fs_in, bump_normal) <= 0 ? -bump_normal : bump_normal;

  mat4 cur_model = model(is_moon, is_boom, -animation_seconds / 4.0);
  vec3 n = normalize((inverse(transpose(view * cur_model)) * vec4(bump_normal, 1)).xyz);

  color = blinn_phong(ka, kd, ks, 1000.0, n, v, l);
 }
