if(isPackage(Auios_FixFov))
	deactivatePackage(Auios_FixFov);
package Auios_FixFov
{
	function GameConnection::CreatePlayer(%cl,%pos)
	{
		if(%cl.idealFov $= "")
		{
			messageClient(%cl, "", "Use /fixFov to set your FOV!");
			%cl.idealFov = 90;
			%cl.fixFov_bypass = false;
		}
		schedule(2, 0, Auios_FixFovTick, %cl);
		return parent::CreatePlayer(%cl, %pos);
	}

	function Auios_FixFovTick(%cl)
	{
		if(!isObject(%cl)) return;
		if(!isObject(%cl.player)) return;
		cancel(%cl.fixFovSchedule);

		if(!%cl.fixFov_bypass)
			if(!%cl.player.fixFov_canZoom)
				%cl.setControlCameraFov(%cl.idealFov);

		%cl.fixFovSchedule = schedule(64, 0, Auios_FixFovTick, %cl);
	}

	function SniperCZoomedImage::onMount(%this, %pl, %slot)
	{
    	parent::onMount(%this, %pl, %slot);

    	// This was from Swol's version. I'm not sure why this is needed
    	// but I'm keeping it.
		schedule(5, 0, Auios_EnableZooming, %pl); 
	}

	function SniperCZoomedImage::onUnMount(%this, %pl, %slot)
	{
    	parent::onUnMount(%this, %pl, %slot);
		%pl.canZoom = false;
	}

	function Auios_EnableZooming(%pl)
	{
	    %pl.fixFov_canZoom = true;	
	}

	function serverCmdFixFov(%cl, %value)
	{
		if(!isObject(%cl)) return;
		if(%value < 90)
			%value = 90;
		if(%value > 120)
			%value = 120;
		%cl.idealFov = %value;
	}

	function serverCmdToggleZoom(%cl, %playerName)
	{
		//Make sure the client is an admin
		if(%cl.isAdmin || %cl.isSuperAdmin || %cl.isLocalConnection() || $Server::LAN || getNumKeyID() == %cl.getBLID())
		{
			if(%playerName $= "")
			{
				if(%cl.fixFov_bypass)
				{
					messageClient(%cl, "", "Disabled zoom bypass");
					%cl.fixFov_bypass = false;
				}
				else
				{
					messageClient(%cl, "", "Enabled zoom bypass");
					%cl.fixFov_bypass = true;
				}
			}
			else
			{
				%target = findClientByName(%playerName);

				if(isObject(%target))
				{
					if(%target.fixFov_bypass)
					{
						messageClient(%cl, "", "Disabled zoom bypass for" SPC %target.name);
						%target.fixFov_bypass = false;
					}
					else
					{
						messageClient(%cl, "", "Enabled zoom bypass for" SPC %target.name);
						%target.fixFov_bypass = true;
					}
				}
			}
		}
	}
};
activatePackage(Auios_FixFov);
