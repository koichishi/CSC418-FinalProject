// Construct the model transformation matrix. The moon should orbit around the
// origin. The other object should stay still.
//
// Inputs:
//   is_moon  whether we're considering the moon
//   is_boom  whether we're considering the boom
//   time  seconds on animation clock
// Returns affine model transformation as 4x4 matrix
//
// expects: identity, rotate_about_y, translate, PI
mat4 model(bool is_moon, bool is_boom, float time)
{
  float sigmoid_time_fall = 1.0 / (1.0 + pow(2.71828, time * 4 + 13.));
  float sigmoid_time_expl = 1.0 / (1.0 + pow(2.71828, time * 4 + 21.));
  float sigmoid_time_blac = 1.0 / (1.0 + pow(2.71828, time * 4 + 28.));
  if (is_moon){
  if (-time * 4 < 20.){
      vec3 pos_t = vec3((2.0 + 0.1 * -time * 4) * (1 - sigmoid_time_fall) + 1.0 , 0, 0);
      return rotate_about_y(2.0 * (time * M_PI)) * (1 - sigmoid_time_fall) * transpose(translate(pos_t)) * uniform_scale(0.3);
    }
  vec3 pos_t = vec3(-1.1 , 0., -0.6);
  return rotate_about_y(time * M_PI) * transpose(translate(pos_t)) * uniform_scale(2.0);
  }
  // this part is for explosure
  if (!is_boom){
    return uniform_scale(1.0 + sigmoid_time_expl * 11);
  }
  else{
    vec3 pos_t = vec3(0., 0., -25.);
    return transpose(translate(pos_t)) * uniform_scale(20.);
  }
}
