REM Root messages
protoc ^
--proto_path=%1Schemes ^
--proto_path=%1Schemes\Messages ^
--csharp_out=%1Generated ^
%1Schemes\Messages\*.proto

REM Avatar Messages
protoc ^
--proto_path=%1Schemes ^
--proto_path=%1Schemes\Messages\Avatar ^
--csharp_out=%1Generated ^
%1Schemes\Messages\Avatar\*.proto

REM PingPong Messages
protoc ^
--proto_path=%1Schemes ^
--proto_path=%1Schemes\Messages\PingPong ^
--csharp_out=%1Generated ^
%1Schemes\Messages\PingPong\*.proto

REM SessionSnapshot Messages
protoc ^
--proto_path=%1Schemes ^
--proto_path=%1Schemes\Messages ^
--proto_path=%1Schemes\Messages\SessionSnapshot ^
--csharp_out=%1Generated ^
%1Schemes\Messages\SessionSnapshot\*.proto