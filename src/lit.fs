
// Add (hard code) an orbiting (point or directional) light to the scene. Light
// the scene using the Blinn-Phong Lighting Model.
//
// Uniforms:
uniform mat4 view;
uniform mat4 proj;
uniform float animation_seconds;
uniform bool is_moon;
// Inputs:
in vec3 sphere_fs_in;
in vec3 normal_fs_in;
in vec4 pos_fs_in; 
in vec4 view_pos_fs_in; 
// Outputs:
out vec3 color;
// expects: PI, blinn_phong
void main()
{
  vec3 ka = vec3(0.0);
  vec3 kd = vec3(0.0);
  if (is_moon) {
    kd = vec3(0.5);
  } 
  else {
    kd = vec3(0.0 ,0.0 ,1.0);
  }
  vec3 ks = vec3(1.0);  
  vec3 v = normalize((view_pos_fs_in).xyz);
  vec3 light = vec3(5, 5, 0);
  // cannot do rotate_about_y() since not declared
  float theta = animation_seconds * M_PI / 4.0;
  mat3 rotation = mat3(cos(theta), 0.0, sin(theta),
                       0.0       , 1.0, 0.0,
                      -sin(theta), 0.0, cos(theta));
  vec3 l = normalize(mat3(view) * rotation * light);

  color = blinn_phong(ka, kd, ks, 2000, normalize(normal_fs_in), v, l);
}
