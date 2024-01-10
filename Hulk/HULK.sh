#!/usr/bin/bash
echo Options:
echo run: Ejecuta el proyecto
echo info: Muestra el archivo Informe
echo finish: Sale de la consola 
echo " "

run () {
    cd Engine
    dotnet run
    echo " "
}

info () {
    cat README.txt
}

finish () { exit; }

while true
do
read selection

case $selection in
run)
run;;
info)
info;;
finish)
finish;;
*)
echo Opción inválida;;
esac

echo " "
done
