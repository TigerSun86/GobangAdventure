set WORKSPACE=..

set GEN_CLIENT=%WORKSPACE%\DesignerConfigs\Luban.ClientServer\Luban.ClientServer.exe
set CONF_ROOT=%WORKSPACE%\DesignerConfigs

%GEN_CLIENT% -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_data_dir %CONF_ROOT%\GenerateDatas\json ^
 --gen_types data_json ^
 -s all 

pause