# Module manifest for module 'Cogito.Build'
# By: Jerome Haltom

@{
    RootModule = 'Cogito.Build'
    ModuleVersion = '2013.9.18.46'
    GUID = 'bf3a4779-4b29-4150-b5e2-c5b6e17a0843'
    Author = 'Jerome Haltom'
    CompanyName = 'Cogito'
    Copyright = '(c) 2013 Jerome Haltom. All rights reserved.'

    RequiredModules = @('Cogito.PowerShell')
    RequiredAssemblies = @('Cogito.Build.dll')

    FunctionsToExport = '*'
    CmdletsToExport = '*'
    VariablesToExport = '*'
    AliasesToExport = '*'

    ModuleList = @("Cogito.Build")
}
