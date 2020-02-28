== About ==

This provides some enhancement to the raw bindings provided by 
Libvirt Bindings (https://libvirt.org/git/?p=libvirt-csharp.git).

== Why ==

Arnaud Champion and Jaromír Červenka have done a wonderful job in providing 
libvirt bindings for .NET, but we thought it would be useful to have a little 
more .NETish interface for working with libvirt. 

This is still a work in progress, so please expect some breaking changes for 
the time being. As soon as this is ready for prime time, we have a nuget package 
planned and will ask the original authors and the libvirt team if they want to 
include it in the project.

Please ignore the MonoDevelop solution for now. We did not maintain it. The
code runs fine on mono (tested with mono 6.8 on RHEL 8).

Contributions are highly appreciated.
 
 == Documentation ==
 
The following code should give you a head start: 

```c#
private static void Connection_DomainEventReceived(object sender, VirDomainEventArgs e)
{
    var domain = (LibvirtDomain)sender;
    Console.WriteLine($"EVENT: {domain.UUID} {domain.Name} {e.EventType.ToString()}");
}

static void Main(string[] args)
{
    using (var connection = LibvirtConnection.Open())
    {
        connection.DomainEventReceived += Connection_DomainEventReceived;

        foreach (var domain in connection.ListDomains(includeDefined: true))
        {
            Console.WriteLine($"{domain.UUID} {domain.Name} {domain.State}");
        }

        Console.WriteLine();
        Console.WriteLine("[ENTER] to exit");
        Console.ReadLine();
    }
}
```

Once again, feel free to contribute!

