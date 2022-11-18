// Given a 3d position as a seed, compute a smooth procedural noise
// value: "Perlin Noise", also known as "Gradient noise".
//
// Inputs:
//   st  3D seed
// Returns a smooth value between (-1,1)
//
// expects: random_direction, smooth_step
float perlin_noise( vec3 st) 
{
  int x = int(floor(st.x));
  int y = int(floor(st.y));
  int z = int(floor(st.z));
  float noise[8];
  int cur_index = 0;
  for (int i = x; i <= x + 1; i++){
    for (int j = y; j <= y + 1; j++){
      for (int k = z; k <= z + 1; k++){
        vec3 diff = vec3(-float(i) + st.x, -float(j) + st.y, -float(k) + st.z);
        if (cur_index <= 7){
            noise[cur_index] = dot(random_direction(vec3(i, j, k)), diff);
            cur_index++;
        }
      }
    }
  }
  float fx = smooth_step(st.x - float(x));
  float fy = smooth_step(st.y - float(y));
  float fz = smooth_step(st.z - float(z));

  float step1 = mix(noise[0], noise[4], fx);
  float step2 = mix(noise[1], noise[5], fx);
  float step3 = mix(noise[2], noise[6], fx);
  float step4 = mix(noise[3], noise[7], fx);
  float step5 = mix(step1, step3, fy);
  float step6 = mix(step2, step4, fy);
  float step7 = mix(step5, step6, fz);

  return step7;
}


