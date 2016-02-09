echo off

if not exist ".\coverage" mkdir ".\coverage"

packages\OpenCover.4.6.519\tools\OpenCover.Console.exe -output:"coverage\backend.xml" -target:run-tests.bat -register:user -log:Error -filter:"%COVERAGE_FILTER_CORE_APPLICATIONSERVICES% %COVERAGE_FILTER_CORE_DOMAINMODEL% %COVERAGE_FILTER_CORE_DOMAINSERVICES% %COVERAGE_FILTER_INFRASTRUCTURE_DATAACCESS% %COVERAGE_FILTER_INFRASTRUCTURE_OPENXML% %COVERAGE_FILTER_PRESENTATION_WEB% %COVERAGE_EXCL_FILTER_PRESENTATION_WEB%"
