using Domain.Statuses;

namespace Tests.Data;

public static class StatusesData
{
    public static Status MainStatus()
        => Status.New(StatusId.New(), "Not Started");
}
