for i in {1..29}; do
    f=$i;
    if [ $i -lt 10 ]
    then
        f="0"$i
    fi
    echo "{\"guid\":\"00000000-8888-1684-912d-611e7eec93$f\",\"name\":\"Floor $i\",\"description\":\"This is a floor.\",\"price\":5,\"textureName\":\"Textures/Floors/00$f\"}" > "$i.floor";
done;
