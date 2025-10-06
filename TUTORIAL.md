# Complete Tutorial: Creating a Visual Studio Extension with a Wizard

## 📋 Table of Contents
1. [Introduction](#introduction)
2. [Prerequisites](#prerequisites)
3. [Project Setup](#project-setup)
4. [Understanding the Project Structure](#understanding-the-project-structure)
5. [Creating the Command Infrastructure](#creating-the-command-infrastructure)
6. [Building the Wizard UI](#building-the-wizard-ui)
7. [Implementing the ViewModel](#implementing-the-viewmodel)
8. [Creating the Command Handler](#creating-the-command-handler)
9. [Configuring the Package](#configuring-the-package)
10. [Project Configuration Explained](#project-configuration-explained)
11. [Building and Testing](#building-and-testing)
12. [Troubleshooting](#troubleshooting)
13. [Next Steps](#next-steps)

---

## Introduction

This tutorial will guide you through creating a Visual Studio 2022 extension that adds a wizard accessible from:
- The **Tools** menu
- **Right-clicking on a Solution** in Solution Explorer

The wizard demonstrates a multi-step interface for collecting user input using WPF and the MVVM pattern.

### What You'll Learn
- How to create a VSIX extension project
- How to add commands to Visual Studio menus
- How to create context menu items
- How to build a WPF wizard with multiple steps
- How to use the MVVM pattern in VS extensions
- How to properly configure old-style .csproj files for VSIX projects

---

## Prerequisites

### Required Software
- **Visual Studio 2022** (Community, Professional, or Enterprise)
- **.NET Framework 4.7.2** or higher
- **Visual Studio SDK** (installed via Visual Studio Installer)

### Required Knowledge
- Basic C# programming
- Basic WPF/XAML understanding
- Familiarity with MVVM pattern (helpful but not required)

### Installing Visual Studio SDK
1. Open **Visual Studio Installer**
2. Click **Modify** on your VS 2022 installation
3. Go to **Workloads** tab
4. Check **Visual Studio extension development**
5. Click **Modify** to install

---

## Project Setup

### Step 1: Create the Project Directory

```powershell
# Create your project directory
mkdir C:\MyProjects\MyVSExtension
cd C:\MyProjects\MyVSExtension
```

### Step 2: Create the Solution File

Create a file named `MyVSExtension.slnx`:

```xml
<Solution>
  <Project Path="MyVSExtension\MyVSExtension.csproj" />
</Solution>
```

### Step 3: Create the Project Directory Structure

```powershell
mkdir MyVSExtension
cd MyVSExtension
mkdir Resources
```

### Step 4: Create the Project File

Create `MyVSExtension.csproj` with the following content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <MinimumVisualStudioVersion>17.0</MinimumVisualStudioVersion>
    <VSToolsPath Condition="'$(VSToolsPath)' == ''">$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)</VSToolsPath>
    <UseCodebase>true</UseCodebase>
    <StartAction>Program</StartAction>
    <StartProgram>$(DevEnvDir)\devenv.exe</StartProgram>
    <StartArguments>/rootsuffix Exp</StartArguments>
  </PropertyGroup>
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <SchemaVersion>2.0</SchemaVersion>
    <ProjectTypeGuids>{82b43b9b-a64c-4715-b499-d71e9ca2bd60};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>
    <ProjectGuid>{YOUR-GUID-HERE}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>MyVSExtension</RootNamespace>
    <AssemblyName>MyVSExtension</AssemblyName>
    <TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion>
    <GeneratePkgDefFile>true</GeneratePkgDefFile>
    <IncludeAssemblyInVSIXContainer>true</IncludeAssemblyInVSIXContainer>
    <IncludeDebugSymbolsInVSIXContainer>true</IncludeDebugSymbolsInVSIXContainer>
    <IncludeDebugSymbolsInLocalVSIXDeployment>true</IncludeDebugSymbolsInLocalVSIXDeployment>
    <CopyBuildOutputToOutputDirectory>true</CopyBuildOutputToOutputDirectory>
    <CopyOutputSymbolsToOutputDirectory>false</CopyOutputSymbolsToOutputDirectory>
    <NuGetPackageImportStamp></NuGetPackageImportStamp>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="LaunchWizardCommand.cs" />
    <Compile Include="MyVSExtensionPackage.cs" />
    <Compile Include="WizardViewModel.cs" />
    <Compile Include="WizardWindow.xaml.cs">
      <DependentUpon>WizardWindow.xaml</DependentUpon>
    </Compile>
  </ItemGroup>
  <ItemGroup>
    <None Include="source.extension.vsixmanifest">
      <SubType>Designer</SubType>
    </None>
  </ItemGroup>
  <ItemGroup>
    <Reference Include="System" />
    <Reference Include="System.Design" />
    <Reference Include="PresentationCore" />
    <Reference Include="PresentationFramework" />
    <Reference Include="System.Xaml" />
    <Reference Include="WindowsBase" />
    <Reference Include="Microsoft.CSharp" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.VisualStudio.Shell.15.0">
      <Version>17.12.40391</Version>
    </PackageReference>
    <PackageReference Include="Microsoft.VisualStudio.Shell.Framework">
      <Version>17.12.40391</Version>
    </PackageReference>
    <PackageReference Include="Microsoft.VisualStudio.Shell.Interop">
      <Version>17.12.40391</Version>
    </PackageReference>
    <PackageReference Include="Microsoft.VSSDK.BuildTools" Version="17.14.2120">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>
  <ItemGroup>
    <Page Include="WizardWindow.xaml">
      <SubType>Designer</SubType>
      <Generator>MSBuild:Compile</Generator>
    </Page>
  </ItemGroup>
  <ItemGroup>
    <VSCTCompile Include="VSCommandTable.vsct">
      <ResourceName>Menus.ctmenu</ResourceName>
      <Generator>VsctGenerator</Generator>
    </VSCTCompile>
  </ItemGroup>
  <ItemGroup>
    <Content Include="Resources\LaunchWizardCommand.png">
      <IncludeInVSIX>true</IncludeInVSIX>
    </Content>
  </ItemGroup>
  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
  <Import Project="$(VSToolsPath)\VSSDK\Microsoft.VsSDK.targets" Condition="'$(VSToolsPath)' != ''" />
</Project>
```

**Important:** Replace `{YOUR-GUID-HERE}` with a new GUID. Generate one using:
```powershell
[guid]::NewGuid().ToString().ToUpper()
```

---

## Understanding the Project Structure

### Key Components

```
MyVSExtension/
├── source.extension.vsixmanifest    # Extension metadata
├── VSCommandTable.vsct              # Command definitions (menus)
├── MyVSExtensionPackage.cs          # Main package class
├── LaunchWizardCommand.cs           # Command handler
├── WizardWindow.xaml                # Wizard UI
├── WizardWindow.xaml.cs             # Wizard code-behind
├── WizardViewModel.cs               # Wizard logic (MVVM)
└── Resources/
    └── LaunchWizardCommand.png      # Command icon
```

### File Purposes

| File | Purpose |
|------|---------|
| **source.extension.vsixmanifest** | Describes your extension: name, version, supported VS versions, icon |
| **VSCommandTable.vsct** | Defines commands and where they appear in VS menus |
| **Package.cs** | Main entry point for your extension, registers services |
| **Command.cs** | Handles command execution when user clicks menu item |
| **WizardWindow.xaml** | WPF user interface for the wizard |
| **WizardViewModel.cs** | Business logic and data for the wizard (MVVM pattern) |

---

## Creating the Command Infrastructure

### Step 1: Create the VSIX Manifest

Create `source.extension.vsixmanifest`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<PackageManifest Version="2.0.0" xmlns="http://schemas.microsoft.com/developer/vsx-schema/2011" xmlns:d="http://schemas.microsoft.com/developer/vsx-schema-design/2011">
  <Metadata>
    <Identity Id="MyVSExtension.12345678-1234-1234-1234-123456789012" Version="1.0" Language="en-US" Publisher="Your Name" />
    <DisplayName>My VS Extension with Wizard</DisplayName>
    <Description>A sample extension that demonstrates a multi-step wizard accessible from Tools menu and Solution context menu.</Description>
  </Metadata>
  <Installation>
    <InstallationTarget Id="Microsoft.VisualStudio.Community" Version="[17.0, 18.0)">
      <ProductArchitecture>amd64</ProductArchitecture>
    </InstallationTarget>
  </Installation>
  <Dependencies>
    <Dependency Id="Microsoft.Framework.NDP" DisplayName="Microsoft .NET Framework" d:Source="Manual" Version="[4.5,)" />
  </Dependencies>
  <Prerequisites>
    <Prerequisite Id="Microsoft.VisualStudio.Component.CoreEditor" Version="[18.0,19.0)" DisplayName="Visual Studio core editor" />
  </Prerequisites>
  <Assets>
    <Asset Type="Microsoft.VisualStudio.VsPackage" d:Source="Project" d:ProjectName="%CurrentProject%" Path="|%CurrentProject%;PkgdefProjectOutputGroup|" />
  </Assets>
</PackageManifest>
```

**Key Elements Explained:**
- `Identity Id`: Unique identifier for your extension (use a new GUID)
- `InstallationTarget`: Specifies which VS versions can install this
- `ProductArchitecture`: amd64 for VS 2022+
- `Assets`: Tells VS this project produces a VsPackage

### Step 2: Create the Command Table (VSCT)

Create `VSCommandTable.vsct`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<CommandTable xmlns="http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable" xmlns:xs="http://www.w3.org/2001/XMLSchema">

  <!-- External header files -->
  <Extern href="stdidcmd.h"/>
  <Extern href="vsshlids.h"/>

  <Commands package="guidMyVSExtensionPackage">
    <!-- Define command groups (where commands are placed) -->
    <Groups>
      <!-- Group for Tools menu -->
      <Group guid="guidMyVSExtensionPackageCmdSet" id="MyToolsMenuGroup" priority="0x0600">
        <Parent guid="guidSHLMainMenu" id="IDM_VS_MENU_TOOLS"/>
      </Group>
      
      <!-- Group for Solution context menu -->
      <Group guid="guidMyVSExtensionPackageCmdSet" id="MySolutionContextMenuGroup" priority="0x0100">
        <Parent guid="guidSHLMainMenu" id="IDM_VS_CTXT_SOLNNODE"/>
      </Group>
    </Groups>

    <!-- Define the actual commands (buttons) -->
    <Buttons>
      <!-- Command in Tools menu -->
      <Button guid="guidMyVSExtensionPackageCmdSet" id="LaunchWizardCommandId" priority="0x0100" type="Button">
        <Parent guid="guidMyVSExtensionPackageCmdSet" id="MyToolsMenuGroup" />
        <Icon guid="guidImages" id="bmpWizard" />
        <Strings>
          <ButtonText>Launch Wizard...</ButtonText>
        </Strings>
      </Button>
      
      <!-- Same command on Solution context menu -->
      <Button guid="guidMyVSExtensionPackageCmdSet" id="LaunchWizardCommandId" priority="0x0100" type="Button">
        <Parent guid="guidMyVSExtensionPackageCmdSet" id="MySolutionContextMenuGroup" />
        <Icon guid="guidImages" id="bmpWizard" />
        <Strings>
          <ButtonText>Launch Wizard...</ButtonText>
        </Strings>
      </Button>
    </Buttons>

    <!-- Define icons for commands -->
    <Bitmaps>
      <Bitmap guid="guidImages" href="Resources\LaunchWizardCommand.png" usedList="bmpWizard"/>
    </Bitmaps>
  </Commands>

  <!-- Define GUIDs and IDs -->
  <Symbols>
    <!-- Package GUID (must match your Package class) -->
    <GuidSymbol name="guidMyVSExtensionPackage" value="{YOUR-PACKAGE-GUID}" />

    <!-- Command set GUID -->
    <GuidSymbol name="guidMyVSExtensionPackageCmdSet" value="{a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d}">
      <IDSymbol name="MyToolsMenuGroup" value="0x1020" />
      <IDSymbol name="MySolutionContextMenuGroup" value="0x1021" />
      <IDSymbol name="LaunchWizardCommandId" value="0x0100" />
    </GuidSymbol>

    <!-- Icon GUID -->
    <GuidSymbol name="guidImages" value="{a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5e}">
      <IDSymbol name="bmpWizard" value="1" />
    </GuidSymbol>
  </Symbols>
</CommandTable>
```

**VSCT Concepts:**
- **Groups**: Container for commands (defines where commands appear)
- **Buttons**: Actual menu items users click
- **Symbols**: GUIDs and IDs that uniquely identify elements
- **Parent**: Links buttons to groups, groups to menus

**Important Parent IDs:**
- `IDM_VS_MENU_TOOLS`: Tools menu
- `IDM_VS_CTXT_SOLNNODE`: Solution node context menu
- `IDM_VS_CTXT_PROJNODE`: Project node context menu

### Step 3: Create the Command Icon

Create a 16x16 PNG icon and save it as `Resources\LaunchWizardCommand.png`. 

You can create one using PowerShell:

```powershell
Add-Type -AssemblyName System.Drawing
$bmp = New-Object System.Drawing.Bitmap(16, 16)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.Clear([System.Drawing.Color]::Transparent)
$g.FillEllipse([System.Drawing.Brushes]::Blue, 2, 2, 12, 12)
$g.DrawString("W", (New-Object System.Drawing.Font("Arial", 8, [System.Drawing.FontStyle]::Bold)), [System.Drawing.Brushes]::White, 3, 2)
$bmp.Save("Resources\LaunchWizardCommand.png", [System.Drawing.Imaging.ImageFormat]::Png)
$g.Dispose()
$bmp.Dispose()
```

---

## Building the Wizard UI

### Step 1: Create WizardWindow.xaml

```xml
<Window x:Class="MyVSExtension.WizardWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:local="clr-namespace:MyVSExtension"
        Title="Project Wizard" Height="500" Width="700"
        WindowStartupLocation="CenterScreen"
        ResizeMode="NoResize">
    <Window.DataContext>
        <local:WizardViewModel />
    </Window.DataContext>
    
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- Header -->
        <Border Grid.Row="0" Background="#0078D4" Padding="20,15">
            <StackPanel>
                <TextBlock Text="{Binding CurrentStepTitle}" 
                           FontSize="20" 
                           FontWeight="Bold" 
                           Foreground="White"/>
                <TextBlock Text="{Binding CurrentStepDescription}" 
                           FontSize="12" 
                           Foreground="White" 
                           Margin="0,5,0,0"/>
            </StackPanel>
        </Border>

        <!-- Content Area -->
        <Border Grid.Row="1" Padding="20" Background="White">
            <ContentControl Content="{Binding CurrentStepContent}">
                <ContentControl.Resources>
                    <!-- Step 1: Welcome -->
                    <DataTemplate DataType="{x:Type local:WelcomeStepViewModel}">
                        <StackPanel>
                            <TextBlock Text="Welcome to the Project Wizard!" 
                                       FontSize="16" 
                                       FontWeight="SemiBold" 
                                       Margin="0,0,0,20"/>
                            <TextBlock TextWrapping="Wrap" 
                                       Margin="0,0,0,10">
                                This wizard will help you set up your project configuration.
                            </TextBlock>
                            <TextBlock TextWrapping="Wrap">
                                Click 'Next' to continue.
                            </TextBlock>
                        </StackPanel>
                    </DataTemplate>

                    <!-- Step 2: Configuration -->
                    <DataTemplate DataType="{x:Type local:ConfigurationStepViewModel}">
                        <StackPanel>
                            <TextBlock Text="Project Name:" 
                                       FontWeight="SemiBold" 
                                       Margin="0,0,0,5"/>
                            <TextBox Text="{Binding ProjectName, UpdateSourceTrigger=PropertyChanged}" 
                                     Padding="5" 
                                     Margin="0,0,0,20"/>

                            <TextBlock Text="Project Type:" 
                                       FontWeight="SemiBold" 
                                       Margin="0,0,0,5"/>
                            <ComboBox SelectedItem="{Binding ProjectType}" 
                                      Padding="5" 
                                      Margin="0,0,0,20">
                                <ComboBoxItem Content="Console Application" IsSelected="True"/>
                                <ComboBoxItem Content="Web Application"/>
                                <ComboBoxItem Content="Class Library"/>
                                <ComboBoxItem Content="Test Project"/>
                            </ComboBox>

                            <CheckBox Content="Include unit tests" 
                                      IsChecked="{Binding IncludeTests}" 
                                      Margin="0,0,0,10"/>
                            <CheckBox Content="Initialize Git repository" 
                                      IsChecked="{Binding InitializeGit}"/>
                        </StackPanel>
                    </DataTemplate>

                    <!-- Step 3: Summary -->
                    <DataTemplate DataType="{x:Type local:SummaryStepViewModel}">
                        <StackPanel>
                            <TextBlock Text="Review Your Configuration" 
                                       FontSize="16" 
                                       FontWeight="SemiBold" 
                                       Margin="0,0,0,20"/>
                            
                            <Grid>
                                <Grid.ColumnDefinitions>
                                    <ColumnDefinition Width="150"/>
                                    <ColumnDefinition Width="*"/>
                                </Grid.ColumnDefinitions>
                                <Grid.RowDefinitions>
                                    <RowDefinition Height="Auto"/>
                                    <RowDefinition Height="Auto"/>
                                    <RowDefinition Height="Auto"/>
                                    <RowDefinition Height="Auto"/>
                                </Grid.RowDefinitions>

                                <TextBlock Grid.Row="0" Grid.Column="0" Text="Project Name:" FontWeight="SemiBold" Margin="0,0,0,10"/>
                                <TextBlock Grid.Row="0" Grid.Column="1" Text="{Binding ProjectName}" Margin="0,0,0,10"/>

                                <TextBlock Grid.Row="1" Grid.Column="0" Text="Project Type:" FontWeight="SemiBold" Margin="0,0,0,10"/>
                                <TextBlock Grid.Row="1" Grid.Column="1" Text="{Binding ProjectType}" Margin="0,0,0,10"/>

                                <TextBlock Grid.Row="2" Grid.Column="0" Text="Include Tests:" FontWeight="SemiBold" Margin="0,0,0,10"/>
                                <TextBlock Grid.Row="2" Grid.Column="1" Text="{Binding IncludeTests}" Margin="0,0,0,10"/>

                                <TextBlock Grid.Row="3" Grid.Column="0" Text="Initialize Git:" FontWeight="SemiBold"/>
                                <TextBlock Grid.Row="3" Grid.Column="1" Text="{Binding InitializeGit}"/>
                            </Grid>

                            <TextBlock Text="Click 'Finish' to create your project configuration." 
                                       Margin="0,20,0,0" 
                                       FontStyle="Italic"/>
                        </StackPanel>
                    </DataTemplate>
                </ContentControl.Resources>
            </ContentControl>
        </Border>

        <!-- Footer Buttons -->
        <Border Grid.Row="2" 
                Background="#F0F0F0" 
                BorderBrush="#D0D0D0" 
                BorderThickness="0,1,0,0" 
                Padding="20,15">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="Auto"/>
                    <ColumnDefinition Width="Auto"/>
                    <ColumnDefinition Width="Auto"/>
                </Grid.ColumnDefinitions>

                <Button Grid.Column="1" 
                        Content="Previous" 
                        Width="80" 
                        Height="30" 
                        Margin="0,0,10,0"
                        Command="{Binding PreviousCommand}"/>
                
                <Button Grid.Column="2" 
                        Content="{Binding NextButtonText}" 
                        Width="80" 
                        Height="30" 
                        Margin="0,0,10,0"
                        Command="{Binding NextCommand}"
                        IsDefault="True"/>
                
                <Button Grid.Column="3" 
                        Content="Cancel" 
                        Width="80" 
                        Height="30"
                        IsCancel="True"/>
            </Grid>
        </Border>
    </Grid>
</Window>
```

**XAML Concepts:**
- **DataTemplate**: Defines how each wizard step looks
- **DataBinding**: `{Binding PropertyName}` connects UI to ViewModel
- **ContentControl**: Displays different content based on current step
- **Command**: Connects buttons to ViewModel methods

### Step 2: Create WizardWindow.xaml.cs

```csharp
using System.Windows;

namespace MyVSExtension
{
    /// <summary>
    /// Interaction logic for WizardWindow.xaml
    /// </summary>
    public partial class WizardWindow : Window
    {
        public WizardWindow()
        {
            InitializeComponent();
        }
    }
}
```

This is minimal because all logic is in the ViewModel (MVVM pattern).

---

## Implementing the ViewModel

Create `WizardViewModel.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyVSExtension
{
    /// <summary>
    /// Base class for view models implementing INotifyPropertyChanged.
    /// This enables two-way data binding between UI and code.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event for a property.
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets a property value and raises PropertyChanged if the value changed.
        /// </summary>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    /// <summary>
    /// Simple implementation of ICommand for binding button clicks to methods.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => canExecute == null || canExecute(parameter);

        public void Execute(object parameter) => execute(parameter);
    }

    /// <summary>
    /// Main wizard view model that manages the wizard steps and navigation.
    /// </summary>
    public class WizardViewModel : ViewModelBase
    {
        private int currentStepIndex;
        private readonly List<WizardStepViewModel> steps;

        public WizardViewModel()
        {
            // Create the wizard steps
            steps = new List<WizardStepViewModel>
            {
                new WelcomeStepViewModel(),
                new ConfigurationStepViewModel(),
                new SummaryStepViewModel()
            };

            currentStepIndex = 0;
            UpdateCurrentStep();

            // Setup commands
            PreviousCommand = new RelayCommand(_ => GoToPreviousStep(), _ => CanGoToPreviousStep());
            NextCommand = new RelayCommand(_ => GoToNextStep(), _ => CanGoToNextStep());
        }

        public ICommand PreviousCommand { get; }
        public ICommand NextCommand { get; }

        public string CurrentStepTitle => CurrentStep?.Title ?? string.Empty;
        public string CurrentStepDescription => CurrentStep?.Description ?? string.Empty;
        public object CurrentStepContent => CurrentStep;
        public string NextButtonText => IsLastStep ? "Finish" : "Next";

        private WizardStepViewModel CurrentStep => currentStepIndex >= 0 && currentStepIndex < steps.Count 
            ? steps[currentStepIndex] 
            : null;

        private bool IsLastStep => currentStepIndex == steps.Count - 1;

        private bool CanGoToPreviousStep() => currentStepIndex > 0;

        private bool CanGoToNextStep() => CurrentStep?.CanMoveNext() ?? false;

        private void GoToPreviousStep()
        {
            if (CanGoToPreviousStep())
            {
                currentStepIndex--;
                UpdateCurrentStep();
            }
        }

        private void GoToNextStep()
        {
            if (!CanGoToNextStep())
                return;

            if (IsLastStep)
            {
                // Finish the wizard
                var window = System.Windows.Application.Current.Windows
                    .OfType<WizardWindow>()
                    .FirstOrDefault();
                
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            else
            {
                currentStepIndex++;
                UpdateCurrentStep();
                UpdateSummaryIfNeeded();
            }
        }

        private void UpdateCurrentStep()
        {
            OnPropertyChanged(nameof(CurrentStepTitle));
            OnPropertyChanged(nameof(CurrentStepDescription));
            OnPropertyChanged(nameof(CurrentStepContent));
            OnPropertyChanged(nameof(NextButtonText));
            CommandManager.InvalidateRequerySuggested();
        }

        private void UpdateSummaryIfNeeded()
        {
            if (CurrentStep is SummaryStepViewModel summary)
            {
                var configStep = steps.OfType<ConfigurationStepViewModel>().FirstOrDefault();
                if (configStep != null)
                {
                    summary.UpdateFromConfiguration(configStep);
                }
            }
        }
    }

    /// <summary>
    /// Base class for wizard step view models.
    /// </summary>
    public abstract class WizardStepViewModel : ViewModelBase
    {
        public abstract string Title { get; }
        public abstract string Description { get; }
        public virtual bool CanMoveNext() => true;
    }

    /// <summary>
    /// Welcome step view model.
    /// </summary>
    public class WelcomeStepViewModel : WizardStepViewModel
    {
        public override string Title => "Welcome";
        public override string Description => "Step 1 of 3";
    }

    /// <summary>
    /// Configuration step view model - collects user input.
    /// </summary>
    public class ConfigurationStepViewModel : WizardStepViewModel
    {
        private string projectName = "MyNewProject";
        private string projectType = "Console Application";
        private bool includeTests = true;
        private bool initializeGit = true;

        public override string Title => "Configuration";
        public override string Description => "Step 2 of 3 - Configure your project";

        public string ProjectName
        {
            get => projectName;
            set => SetProperty(ref projectName, value);
        }

        public string ProjectType
        {
            get => projectType;
            set => SetProperty(ref projectType, value);
        }

        public bool IncludeTests
        {
            get => includeTests;
            set => SetProperty(ref includeTests, value);
        }

        public bool InitializeGit
        {
            get => initializeGit;
            set => SetProperty(ref initializeGit, value);
        }

        // Only allow moving to next step if project name is provided
        public override bool CanMoveNext() => !string.IsNullOrWhiteSpace(ProjectName);
    }

    /// <summary>
    /// Summary step view model - shows review of user selections.
    /// </summary>
    public class SummaryStepViewModel : WizardStepViewModel
    {
        private string projectName;
        private string projectType;
        private bool includeTests;
        private bool initializeGit;

        public override string Title => "Summary";
        public override string Description => "Step 3 of 3 - Review your configuration";

        public string ProjectName
        {
            get => projectName;
            set => SetProperty(ref projectName, value);
        }

        public string ProjectType
        {
            get => projectType;
            set => SetProperty(ref projectType, value);
        }

        public bool IncludeTests
        {
            get => includeTests;
            set => SetProperty(ref includeTests, value);
        }

        public bool InitializeGit
        {
            get => initializeGit;
            set => SetProperty(ref initializeGit, value);
        }

        public void UpdateFromConfiguration(ConfigurationStepViewModel config)
        {
            ProjectName = config.ProjectName;
            ProjectType = config.ProjectType;
            IncludeTests = config.IncludeTests;
            InitializeGit = config.InitializeGit;
        }
    }
}
```

**ViewModel Pattern Explained:**
- **ViewModelBase**: Implements `INotifyPropertyChanged` for data binding
- **RelayCommand**: Implements `ICommand` to connect UI buttons to methods
- **WizardViewModel**: Coordinates navigation between steps
- **Step ViewModels**: Each wizard step has its own ViewModel with properties

---

## Creating the Command Handler

Create `LaunchWizardCommand.cs`:

```csharp
using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace MyVSExtension
{
    /// <summary>
    /// Command handler for launching the wizard.
    /// This is invoked when the user clicks the menu item.
    /// </summary>
    internal sealed class LaunchWizardCommand
    {
        /// <summary>
        /// Command ID (must match the ID in VSCommandTable.vsct).
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group GUID (must match the GUID in VSCommandTable.vsct).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d");

        /// <summary>
        /// VS Package that provides this command.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the LaunchWizardCommand class.
        /// Registers the command with Visual Studio.
        /// </summary>
        private LaunchWizardCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the singleton instance of the command.
        /// </summary>
        public static LaunchWizardCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IAsyncServiceProvider ServiceProvider => this.package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// Called from the package's InitializeAsync method.
        /// </summary>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - commands require the UI thread
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            // Get the command service
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            
            // Create the command instance
            Instance = new LaunchWizardCommand(package, commandService);
        }

        /// <summary>
        /// This method is called when the menu item is clicked.
        /// It shows the wizard dialog.
        /// </summary>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Create and show the wizard window
            var wizardWindow = new WizardWindow();
            var result = wizardWindow.ShowDialog();

            if (result == true)
            {
                // User clicked Finish - process the wizard data
                var viewModel = wizardWindow.DataContext as WizardViewModel;
                
                // TODO: Do something with the wizard data
                // For now, just show a message
                VsShellUtilities.ShowMessageBox(
                    this.package,
                    "Wizard completed successfully!",
                    "Success",
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }
    }
}
```

**Command Handler Concepts:**
- **CommandID**: Unique identifier (GUID + int) that links code to VSCT
- **OleMenuCommand**: VS-specific command that can be added to menus
- **ThreadHelper**: Ensures code runs on the UI thread (required for VS APIs)
- **ShowDialog()**: Displays the wizard as a modal dialog

---

## Configuring the Package

Create `MyVSExtensionPackage.cs`:

```csharp
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace MyVSExtension
{
    /// <summary>
    /// This is the main package class that implements the VS package.
    /// It's the entry point for the extension.
    /// </summary>
    /// <remarks>
    /// The PackageRegistration attribute tells VS this is a package.
    /// The Guid attribute provides a unique identifier.
    /// The ProvideMenuResource attribute links to the command table.
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(MyVSExtensionPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class MyVSExtensionPackage : AsyncPackage
    {
        /// <summary>
        /// Package GUID string (must match GUID in VSCommandTable.vsct and .csproj).
        /// </summary>
        public const string PackageGuidString = "YOUR-PACKAGE-GUID-HERE";

        #region Package Members

        /// <summary>
        /// Initialization of the package.
        /// This method is called when the package is loaded by Visual Studio.
        /// </summary>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread.
            // Switch to the UI thread for any initialization that requires it.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // Initialize the command
            await LaunchWizardCommand.InitializeAsync(this);
        }

        #endregion
    }
}
```

**Package Attributes Explained:**
- `[PackageRegistration]`: Registers this class as a VS package
- `[Guid]`: Unique identifier for your package (use the same GUID from your .csproj)
- `[ProvideMenuResource]`: Links to the compiled command table resource

**Important:** Replace `YOUR-PACKAGE-GUID-HERE` with the GUID you used in the .csproj file.

---

## Project Configuration Explained

### Why Old-Style .csproj Format?

Visual Studio extensions use **old-style .csproj format** (not SDK-style) because:
1. The VSSDK build tools were designed for this format
2. The `Microsoft.VsSDK.targets` import only works with old-style projects
3. It provides better control over VSIX packaging

### Critical Project Properties

```xml
<ProjectTypeGuids>{82b43b9b-a64c-4715-b499-d71e9ca2bd60};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>
```
- First GUID: Identifies this as a VSIX project
- Second GUID: Identifies this as a C# project

```xml
<GeneratePkgDefFile>true</GeneratePkgDefFile>
```
- Generates the .pkgdef file needed for package registration

```xml
<IncludeAssemblyInVSIXContainer>true</IncludeAssemblyInVSIXContainer>
```
- Includes your DLL in the VSIX package

```xml
<StartAction>Program</StartAction>
<StartProgram>$(DevEnvDir)\devenv.exe</StartProgram>
<StartArguments>/rootsuffix Exp</StartArguments>
```
- Configures F5 debugging to launch VS experimental instance

### Critical Imports

```xml
<Import Project="$(VSToolsPath)\VSSDK\Microsoft.VsSDK.targets" />
```
- **This is what creates the VSIX file!**
- Without this, you only get a DLL, not an installable extension

### NuGet Packages Explained

```xml
<PackageReference Include="Microsoft.VisualStudio.Shell.15.0">
```
- Provides core VS extension types like `AsyncPackage`

```xml
<PackageReference Include="Microsoft.VSSDK.BuildTools">
```
- Provides build tools that compile .vsct files and create VSIX packages

---

## Building and Testing

### Step 1: Restore NuGet Packages

Open PowerShell in your project directory:

```powershell
cd C:\MyProjects\MyVSExtension
dotnet restore MyVSExtension.slnx
```

### Step 2: Open in Visual Studio

1. Open Visual Studio 2022
2. File → Open → Project/Solution
3. Select `MyVSExtension.slnx`

### Step 3: Build the Project

1. Build → Rebuild Solution (Ctrl+Shift+B)
2. Check the Output window for any errors
3. Look for `MyVSExtension.vsix` in `bin\Debug\` folder

### Step 4: Debug the Extension

1. Press **F5** or click **Debug → Start Debugging**
2. Visual Studio will launch an **Experimental Instance**
3. In the experimental instance:
   - Create or open any solution
   - Go to **Tools → Launch Wizard...**
   - OR right-click on the **Solution node** in Solution Explorer → **Launch Wizard...**

### Step 5: Test the Wizard

1. Click through all wizard steps
2. Verify Previous/Next buttons work
3. Try entering different values
4. Click Finish on the last step
5. Verify the success message appears

### Verifying VSIX Creation

Check that the VSIX was created:

```powershell
Test-Path "bin\Debug\MyVSExtension.vsix"
# Should return: True
```

List the contents:

```powershell
# VSIX is just a ZIP file
Expand-Archive -Path "bin\Debug\MyVSExtension.vsix" -DestinationPath "bin\Debug\vsix-contents" -Force
Get-ChildItem "bin\Debug\vsix-contents" -Recurse
```

You should see:
- `extension.vsixmanifest`
- `MyVSExtension.dll`
- `MyVSExtension.pkgdef`
- Resources folder with your icon

---

## Troubleshooting

### Problem: "Cannot run because it's a class library"

**Cause:** Visual Studio doesn't know how to debug the extension.

**Solution:** Add these properties to your .csproj:
```xml
<PropertyGroup>
  <StartAction>Program</StartAction>
  <StartProgram>$(DevEnvDir)\devenv.exe</StartProgram>
  <StartArguments>/rootsuffix Exp</StartArguments>
</PropertyGroup>
```

### Problem: "Your project doesn't list 'win' as a RuntimeIdentifier"

**Cause:** Using SDK-style .csproj format with VSIX projects causes NuGet restore issues.

**Solution:** 
1. Convert to old-style .csproj format (remove `Sdk="Microsoft.NET.Sdk"` attribute)
2. Use old-style project format as shown in this tutorial
3. Add `<NuGetPackageImportStamp></NuGetPackageImportStamp>` to properties

**Why this happens:** The VSSDK build tools were designed for old-style projects and don't work properly with SDK-style PackageReference resolution.

### Problem: Only DLL Created, No VSIX File

**Cause:** Missing the critical VSSDK targets import.

**Solution:** Check that your .csproj has this import **at the bottom**:
```xml
<Import Project="$(VSToolsPath)\VSSDK\Microsoft.VsSDK.targets" Condition="'$(VSToolsPath)' != ''" />
```

**This import is what actually creates the VSIX package!** Without it, you only get a DLL.

### Problem: "AsyncPackage could not be found" or "Microsoft.VisualStudio not found"

**Cause:** Missing Visual Studio SDK NuGet packages.

**Solution:** Add these NuGet packages:
```xml
<PackageReference Include="Microsoft.VisualStudio.Shell.15.0">
  <Version>17.12.40391</Version>
</PackageReference>
<PackageReference Include="Microsoft.VisualStudio.Shell.Framework">
  <Version>17.12.40391</Version>
</PackageReference>
<PackageReference Include="Microsoft.VisualStudio.Shell.Interop">
  <Version>17.12.40391</Version>
</PackageReference>
```

**Note:** Don't use `Microsoft.VisualStudio.SDK` package with old-style projects - use the individual Shell packages instead.

### Problem: "Unable to find package Community.VisualStudio.Toolkit with version X"

**Cause:** Trying to use Community toolkit version that doesn't exist yet.

**Solution:** 
1. Check available versions on NuGet.org
2. Use the actual latest version (e.g., 17.0.533 instead of 17.0.570)
3. **OR better yet:** Use direct Microsoft.VisualStudio.Shell packages as shown in this tutorial (simpler for old-style projects)

### Problem: Community.VisualStudio.Toolkit imports not working in old-style project

**Cause:** The toolkit's build props/targets aren't automatically imported in old-style .csproj files.

**Solution:** Either:
1. **Recommended:** Use direct Microsoft.VisualStudio.Shell packages (simpler)
2. **OR** Manually import toolkit props/targets (complex, not recommended)

**Why we recommend direct packages:** The Community toolkit is optimized for SDK-style projects. For old-style VSIX projects, direct Microsoft packages are more reliable.

### Problem: Menu Item Doesn't Appear

**Checklist:**
1. Verify GUIDs match between Package.cs, VSCommandTable.vsct, and .csproj
2. Check that `[ProvideMenuResource("Menus.ctmenu", 1)]` attribute is on package class
3. Verify command is being initialized in `InitializeAsync`
4. Rebuild and restart experimental instance
5. Reset experimental instance: `devenv /rootsuffix Exp /ResetSettings`

**Debug tip:** Add a breakpoint in `LaunchWizardCommand.InitializeAsync` to verify it's being called.

### Problem: Icon Not Showing

**Solution:**
1. Verify icon is 16x16 PNG
2. Check .csproj includes icon with `<IncludeInVSIX>true</IncludeInVSIX>`
3. Verify path in VSCommandTable.vsct matches actual file location
4. Rebuild project

### Problem: Extension Not Loading in Experimental Instance

**Solution:**
1. Tools → Extensions → Manage Extensions
2. Check if extension is listed but disabled
3. Close experimental instance
4. Delete: `%LocalAppData%\Microsoft\VisualStudio\17.0_[xxxxx]Exp\Extensions\`
5. Rebuild and run again

**Nuclear option:** Reset the entire experimental instance:
```powershell
devenv /rootsuffix Exp /ResetSettings
```

### Problem: Build succeeds but VSIX not in bin folder

**Checklist:**
1. Verify `<GeneratePkgDefFile>true</GeneratePkgDefFile>` in .csproj
2. Verify `<IncludeAssemblyInVSIXContainer>true</IncludeAssemblyInVSIXContainer>` in .csproj
3. Check that source.extension.vsixmanifest has proper Assets section
4. Ensure Microsoft.VSSDK.BuildTools package is installed
5. Look in Output window for build errors during VSIX creation

### Problem: "InitializeComponent does not exist" in WizardWindow.xaml.cs

**Cause:** XAML hasn't been compiled yet - this is normal before first build.

**Solution:** Just build the project. The XAML compiler generates InitializeComponent() during build. This error will disappear after successful build.

### Problem: Extension loads but wizard window is blank

**Checklist:**
1. Verify WizardWindow.xaml is marked as `<Page>` in .csproj
2. Check that `<Generator>MSBuild:Compile</Generator>` is set for XAML
3. Verify ViewModel is set as DataContext in XAML
4. Check for binding errors in Output window → Debug
5. Ensure all ViewModel properties have proper getters

### Common Mistakes to Avoid

1. **Using SDK-style .csproj** - Always use old-style format for VSIX projects
2. **Forgetting Microsoft.VsSDK.targets import** - This creates the VSIX
3. **GUID mismatches** - Package GUID must match in .csproj, Package.cs, and vsixmanifest
4. **Not calling Command.InitializeAsync** - Commands won't register
5. **Wrong TargetFramework** - Use .NET Framework 4.7.2, not .NET Core/.NET 5+

---

## Next Steps

### Enhance Your Wizard

1. **Add More Steps:**
   ```csharp
   steps.Add(new AdditionalOptionsStepViewModel());
   ```

2. **Persist User Choices:**
   ```csharp
   // Use VS settings store
   var settingsManager = await package.GetServiceAsync(typeof(SVsSettingsManager));
   ```

3. **Access Solution/Project:**
   ```csharp
   var dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
   var solution = dte.Solution;
   ```

4. **Create Files Based on Wizard Input:**
   ```csharp
   var project = dte.Solution.Projects.Item(1);
   project.ProjectItems.AddFromTemplate(templatePath, fileName);
   ```

### Publish Your Extension

1. **Create Publisher Account:**
   - Go to https://marketplace.visualstudio.com/manage
   - Create a publisher

2. **Update Manifest:**
   ```xml
   <Identity Publisher="YourPublisherName" />
   <Icon>Resources\ExtensionIcon.png</Icon>
   <PreviewImage>Resources\Preview.png</PreviewImage>
   <Tags>wizard, tools, productivity</Tags>
   ```

3. **Upload to Marketplace:**
   - Build in Release mode
   - Upload the VSIX file
   - Add description, screenshots, changelog

### Learn More

**Official Documentation:**
- [Visual Studio SDK](https://learn.microsoft.com/en-us/visualstudio/extensibility/)
- [VSIX Extension](https://learn.microsoft.com/en-us/visualstudio/extensibility/anatomy-of-a-vsix-package)
- [Command Table (VSCT)](https://learn.microsoft.com/en-us/visualstudio/extensibility/internals/visual-studio-command-table-dot-vsct-files)

**Sample Extensions:**
- [VS SDK Samples on GitHub](https://github.com/Microsoft/VSSDK-Extensibility-Samples)
- [Mads Kristensen's Extensions](https://github.com/madskristensen)

**Community:**
- [Gitter Chat for VS Extensibility](https://gitter.im/Microsoft/extendvs)
- [Stack Overflow - visual-studio-extensions tag](https://stackoverflow.com/questions/tagged/visual-studio-extensions)

---

## Summary

You've learned how to:
- ✅ Create a VS extension project with old-style .csproj
- ✅ Define commands in VSCT files
- ✅ Add commands to Tools menu and context menus
- ✅ Build multi-step WPF wizards using MVVM
- ✅ Handle command execution
- ✅ Configure project for VSIX generation
- ✅ Debug extensions in experimental instance
- ✅ Package extensions for distribution

### Key Takeaways

1. **Old-style .csproj is required** for VSIX projects
2. **VSCT defines menu structure**, code handles execution
3. **GUIDs must match** across Package, VSCT, and project file
4. **MVVM pattern** separates UI from logic
5. **Microsoft.VsSDK.targets** import creates the VSIX

Happy extending Visual Studio! 🎉

---

*Tutorial created: October 2025*
*Target: Visual Studio 2022*
*Framework: .NET Framework 4.7.2*
