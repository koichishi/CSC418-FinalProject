// Generate a procedural planet and orbiting moon. Use layers of (improved)
// Perlin noise to generate planetary features such as vegetation, gaseous
// clouds, mountains, valleys, ice caps, rivers, oceans. Don't forget about the
// moon. Use `animation_seconds` in your noise input to create (periodic)
// temporal effects.
//
// Uniforms:
uniform mat4 view;
uniform mat4 proj;
uniform float animation_seconds;
uniform bool is_moon;
uniform bool is_boom;
uniform float width_window;
uniform float height_window;
// Inputs:
in vec3 sphere_fs_in;
in vec3 normal_fs_in;
in vec4 pos_fs_in;
in vec4 view_pos_fs_in;
// Outputs:
out vec3 color;
// expects: model, blinn_phong, bump_height, bump_position,
// improved_perlin_noise, tangent

const vec3 light_center = vec3(0., 0., 0.);

const float maxDist = 1e30;

struct Ray
{
  vec3 o;
  vec3 d;
};

struct Cone
{
  // direction of radius grow fastest
  vec3 v;
  // color vector
  vec3 c;
  // angle (theta)
  float a;
};

Cone lights[10];

float hit_cone(Cone light, Ray ray)
{
  float cosa = cos(light.a);
  float cos2a = cosa * cosa;
  float dv = dot(ray.d, light.v);
  vec3 co = ray.o - light_center;
  float cov = dot(co, light.v);
  float dco = dot(ray.d, co);

  float a = dv * dv - cos2a;
  float b = 2.0 * (dv * cov - dco * cos2a);
  float c = cov * cov - dot(co, co) * cos2a;

  float delta = b * b - 4.0 * a * c;
  float sqrt_delta = pow(delta, 0.5);

  float t1 = (-b - sqrt_delta) / (2.0 * a);
  float t2 = (-b + sqrt_delta) / (2.0 * a);
  float t = -1.;

  if (delta >= 1e-6) {
    if (t2 < 1e-6) {
      return -1.;
    }
    else if (t1 < 1e-6 && t2 < maxDist) {
      t = t2;
    }
    else if (t1 < maxDist){
      t = t1;
    }
  }
  return t;
}

vec3 cone_color(Ray ray)
{
  vec3 color = vec3(0.0, 0.0, 0.0);

  for (int i = 0; i < 10; i++)
  {
    float t = hit_cone(lights[i], ray);
    if (t != -1.)
    {
      // need to check whether the intersection is on the other side too
      if (animation_seconds < 22. && (dot((ray.o + t * ray.d - light_center), lights[i].v) >= 0))
        color += 0.1 * lights[i].c;
      
      // switch to disco light
      else if (animation_seconds > 22.)
        color += 0.1 * lights[i].c;
    }
  }
  return color;
}

// Structure adapted from: https://www.shadertoy.com/view/XstSRX
void setUpLights(float collision_t)
{
  // set up the position
  float rotate_t = animation_seconds - collision_t;
  mat4 m = rotate_about_z(2.0 * M_PI * rotate_t * 0.02) *
    rotate_about_x(2.0 * M_PI * rotate_t * 0.1) *
    rotate_about_x(2.0 * M_PI * rotate_t * 0.1);
  lights[0].v = normalize(m * vec4(1., 0., 0., 0.)).xyz;
  lights[1].v = normalize(m * m * vec4(1., 0., 0., 0.)).xyz;
  lights[2].v = normalize(m * m * m * vec4(1., 0., 0., 0.)).xyz;
  lights[3].v = normalize(m * m * m * m * vec4(1., 0., 0., 0.)).xyz;
  lights[4].v = normalize(m * m * m * m * m * vec4(1., 0., 0., 0.)).xyz;
  lights[5].v = normalize(m * m * m * m * m * m * vec4(1., 0., 0., 0.)).xyz;
  lights[6].v = normalize(m * m * m * m * m * m * m * vec4(1., 0., 0., 0.)).xyz;
  lights[7].v = normalize(m * m * m * m * m * m * m * m * vec4(1., 0., 0., 0.)).xyz;
  lights[8].v = normalize(m * m * m * m * m * m * m * m * m * vec4(1., 0., 0., 0.)).xyz;
  lights[9].v = normalize(m * m * m * m * m * m * m * m * m * m * vec4(1., 0., 0., 0.)).xyz;
  for (int i = 0; i < 10; i++)
  {
    // set up the radius
    lights[i].a = M_PI / 40.;
    // set up the color
    if (i % 5 == 0)
      lights[i].c = vec3(1., 0, 1.);
    if (i % 5 == 1)
      lights[i].c = vec3(0., 1., 1.);
    if (i % 5 == 2)
      lights[i].c = vec3(0., 1., 0.);
    if (i % 5 == 3)
      lights[i].c = vec3(0., 0., 1.);
    if (i % 5 == 4)
      lights[i].c = vec3(1., 0., 0.);
  }
}       


void main()
{
  // land
  float sigmoid_time1 = 1.0 / (1.0 + pow(2.71828, -animation_seconds + 4.));
  // cloud
  float sigmoid_time2 = 1.0 / (1.0 + pow(2.71828, -animation_seconds + 7.));
  // civilization
  float sigmoid_time3 = 1.0 / (1.0 + pow(2.71828, -animation_seconds + 10.));
  // explosure
  float sigmoid_time_expl = 1.0 / (1.0 + pow(2.71828, (-animation_seconds + 17.) * 3.));
  // black
  float sigmoid_time_black = 1.0 / (1.0 + pow(2.71828, (-animation_seconds + 21.) * 5.));
  vec3 ka = vec3(0.05);
  vec3 kd;
  if (is_moon) {
    kd = vec3(0.5);
  }
  else if (!is_boom) {
    kd = vec3(0.1, 0.1, 1.0);
  }
  else {
    kd = vec3(0.03);
  }
  vec3 ks = vec3(0.1);
  float p = 1000.0;
  float theta = animation_seconds * M_PI / 4.0;
  mat4 rotation = mat4(cos(theta), 0.0, sin(theta), 0.0,
    0.0, 1.0, 0.0, 0.0,
    -sin(theta), 0.0, cos(theta), 0.0,
    0.0, 0.0, 0.0, 1.0);

  vec3 bump_sphere = bump_position(is_moon, sphere_fs_in);
  vec4 bump_view = view * model(is_moon, is_boom, -animation_seconds / 4.0) * vec4(bump_sphere, 1.0);

  vec3 l = normalize((view * rotation * vec4(1, 1, 1, 0)).xyz);
  vec3 v = normalize((view * bump_view).xyz);
  // Bump normal
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

  // Earth color
  if (!is_moon && !is_boom && bump_height(is_moon, sphere_fs_in) > 0) {
    kd = kd + (vec3(0.4, 0.8, 0.4) - kd) * sigmoid_time1;
    float land_noise = 0;
    for (int i = 0; i < 3; i++) {
      land_noise += (improved_perlin_noise(sphere_fs_in + bump_height(is_moon, sphere_fs_in)));
    }
    kd = kd + ((mix(kd, vec3(0.8, 0.8, 0.6), land_noise)) - kd) * sigmoid_time1;
  }
  color = blinn_phong(
    ka, kd, ks, p,
    n,
    v,
    l);

  // Cloud Color
  vec3 f = vec3(2, 3, 3);
  if (!is_moon && !is_boom) {
    float cloud_noise = improved_perlin_noise(f * vec3(rotate_about_y(animation_seconds / 4.0) * vec4(sphere_fs_in, 1)));
    if (cloud_noise > 0.1) {
      ka = vec3(0.0);
      kd = vec3(1.0);
      vec3 cloud_color = blinn_phong(
        ka, kd, ks, p,
        normalize(normal_fs_in),
        v,
        l); // normal_fs_in since over the land
      color += (sigmoid_time2 * cloud_color);
    }
  }

  // City Light
  f = vec3(100, 100, 100);
  if (!is_moon && !is_boom) {
    float city_noise = improved_perlin_noise(f * sphere_fs_in);
    if (city_noise > 0.35 && bump_height(is_moon, sphere_fs_in) > 0.08) {
      ka = vec3(1.0);
      kd = vec3(1.0);
      vec3 cloud_color = blinn_phong(
        ka, kd, ks, p,
        n,
        v,
        l);
      color += (sigmoid_time3 * city_noise);
    }
  }

  vec3 explosure_color = vec3(1.0, 0.1, 0.1);
  // Explosure Color
  f = vec3(10, 40, 3);
  if (!is_moon && !is_boom) {
    float explosure_noise = improved_perlin_noise(f * vec3(rotate_about_y(-animation_seconds / 4.0) * vec4(sphere_fs_in, 1)));
    if (explosure_noise > 0) {
      ka = vec3(0.4, 0.0, 0.0);
      kd = vec3(.5, 0.1, 0.1);
      vec3 explosure_color1 = blinn_phong(
        ka, kd, ks, p,
        normalize(normal_fs_in),
        v,
        l); // normal_fs_in since over the land
      explosure_color = (sigmoid_time2 * explosure_color1);
    }
  }

  // Change to explosure color
  // Fade to black
  if (!is_moon && !is_boom){
    color = (1 - sigmoid_time_expl) * color + sigmoid_time_expl * explosure_color;
    color *= (1 - sigmoid_time_black);
  }
  else if (animation_seconds > 20.){
    color = vec3(1, 0.3, 0.3);
    float land_noise = 0;
    for (int i = 0; i < 3; i++) {
      land_noise += (improved_perlin_noise(sphere_fs_in + bump_height(is_moon, sphere_fs_in)));
    }
    color = mix(color, vec3(0.6, 0.1, 0.2), land_noise);
  }

  // Cone lights
  setUpLights(16.);

  Ray ray;
  mat4 inv_proj = inverse(proj);
  mat4 inv_view = inverse(view);

  // shift to clip coordinate (-1. ~ 1.)
  vec4 clipCoord = vec4(
      (gl_FragCoord.x / width_window - 0.5) * 2.0,
      (gl_FragCoord.y / height_window - 0.5) * 2.0,
      (gl_FragCoord.z - 0.5) * 2.0,
      1.0);
  vec4 temp = inv_view * inv_proj * clipCoord;
  vec3 worldCoord = temp.xyz / temp.w;

  // transform to world coordinate
  ray.o = worldCoord;

  // big screen world coordinate (consider a flat one)
  float screen_angle = tanh(height_window / width_window);
  float x_dist = cos(screen_angle) * 20.; // 20: radius of the screen ball
  float y_dist = sin(screen_angle) * 20.; 
  vec3 screenCoord = vec3 (clipCoord.x * x_dist, clipCoord.y * y_dist, light_center.z);
  ray.d = normalize(vec4(screenCoord - ray.o, 0.0)).xyz;

  // background color
  Ray ray2;
  ray2.o = ray.o;
  ray2.d = normalize(-ray2.o);

  if (is_boom && animation_seconds > 16.){
    float factor = (animation_seconds - 16.);
    factor = min(factor, 4.);
    color = cone_color(ray) * factor;
    if (animation_seconds > 22.){
        vec3 golden_experience = cone_color(ray2)
        * sin(0.01*(animation_seconds-22.)*dot(ray2.o, vec3(ray2.o.x, ray2.o.y, 1.)))
        * cos(0.01*(animation_seconds-22.)*dot(ray2.o, vec3(ray2.o.y, ray2.o.x, 1.)))
        + vec3(sin(animation_seconds/2) + 1., sin(animation_seconds*2) + 1., sin(animation_seconds+2) + 1.) / 4.    ;
        golden_experience[0] = golden_experience[0] * golden_experience[0];
        golden_experience[1] = golden_experience[1] * golden_experience[1];
        golden_experience[2] = golden_experience[2] * golden_experience[2];
        color += golden_experience;
    }
  }
}
